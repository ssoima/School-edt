/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

#include <stdio.h>
#include <unistd.h>
#include <stdlib.h>
#include <errno.h>
#include <string.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <fcntl.h>
#include <signal.h>

#define MAX(a, b) ((a) < (b) ? (b) : (a))
#define MIN(a, b) ((a) < (b) ? (a) : (b))

#define BUFFLEN 1024

static sig_atomic_t run = 1;

static void
printHelp(const char *name)
{
	fprintf(stdout,"Usage: %s <local port> <remote_ip> <remote_port>\n\n",
									name);
}

void sighandler(int signo)
{
	switch (signo)
	{
	case SIGINT:
	case SIGTERM:
		fprintf(stderr, "Received signal %d, shutting down...\n",
							signo);
		run = 0;
		break;
	default:
		fprintf(stderr, "Received signal %d, ignoring...\n",
								signo);
	}
}

static char * sanitize(const char *str, size_t len)
{
	static char buffer[BUFFLEN+2];
	unsigned int i,j;

	if (len > BUFFLEN) {
		fprintf(stderr, "ERROR: message too long\n");
		return NULL;
	}

	memset(buffer, 0, sizeof(buffer));

	for (i=0,j=0; i<len; i++) {
		if (str[i] < 0x20 || str[i] > 0x7f)
			continue;
		buffer[j] = str[i];
		j++;
	}

	/* Add a line feed at the end of the string */
	buffer[j] = 0x0a;

	return buffer;
}

int
main(int argc, char **argv)
{
	struct sockaddr_in sal, sar, safrom;
	int rport, lport, sd, maxfd, ret, len;
	char buffer[BUFFLEN];
	char *ptr;
	socklen_t slen;
	fd_set rfd, rfds;

	signal(SIGINT, sighandler);
	signal(SIGTERM, sighandler);

	/* Check arguments and printHelp/exit when something is wrong */
	if (argc != 4) {
		printHelp(argv[0]);
		exit(1);
	}
	
	/* Initialize everything to safe values */
	slen = sizeof(safrom);
	memset(&sal, 0, sizeof(sal));
	memset(&sar, 0, sizeof(sar));
	memset(&safrom, 0, sizeof(safrom));

	/* Read options from command line and initialize address structs*/
	lport = atoi(argv[1]);
	rport = atoi(argv[3]);

	if (0 == inet_aton(argv[2], &sar.sin_addr)) {
		fprintf(stderr, "invalid IP address: %s\n", argv[2]);
		exit(-1);
	}
	sar.sin_port = htons(rport);
	sar.sin_family = AF_INET;

	sal.sin_addr.s_addr = INADDR_ANY;
	sal.sin_port = htons(lport);
	sal.sin_family = AF_INET;

	/* Create a new UDP socket */
	if (0 > (sd = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP))) {
		perror("socket() failed");
		exit(1);
	}

	/* Bind the socket to the local port to
	 * - to be sure others can reach us on that port and
	 * - to force the OS to use that port as sorurce port when sending. */
	if (0 > bind(sd, (struct sockaddr *)&sal, sizeof(sal))) {
		perror("bind() failed");
		exit(1);
	}

	/* Prepare the file descriptor set for select() */
	FD_ZERO(&rfds);
	FD_SET(sd, &rfds);
	FD_SET(STDIN_FILENO, &rfds);
	maxfd = MAX(sd, STDIN_FILENO);

	/* Event loop */
	while (run) {
		/* Initializing the buffer to 0 is probably a good idea */
		memset(buffer, 0, sizeof(buffer));

		/* Copy the read file descriptor set from the backup */
		rfd = rfds;

		/* Call select and wait for something interesting to happen */
		ret = select(maxfd+1, &rfd, NULL, NULL, NULL);
		if (0 > ret) {
			if (errno == EINTR)
				continue;	// that's ok

			perror("select() failed");
			exit(1);
		}
		if (0 == ret)
			continue;		// should not happen here

		/* If something arrives at the socket, read from the socket and
		 * print it on stdout */
		if (FD_ISSET(sd, &rfd)) {
			len = recvfrom(sd, buffer, sizeof(buffer),
				O_NONBLOCK, (struct sockaddr *)&safrom,
				&slen);

			if (0 > len) {
				if (errno == EAGAIN)
					continue;
				perror("recvfrom() failed");
				exit(1);
			}

			if (len == 0)
				continue;
	
			/* Remove non-ASCII characters and ASCII control
			 * characters from the received string. */
			ptr = sanitize(buffer, len);

			fprintf(stdout, "%s:%d >> %s",
				inet_ntoa(safrom.sin_addr),
				ntohs(safrom.sin_port),
				ptr);
		}
		
		/* If someone is typing, read from stdin and send it to the
		 * remote node through our socket */
		if (FD_ISSET(STDIN_FILENO, &rfd)) {
			if (NULL == fgets(buffer, BUFFLEN, stdin))
				continue;
			
			/* Remove non-ASCII characters and ASCII control
			 * characters from the received string. */
			ptr = sanitize(buffer, strlen(buffer));

			ret = sendto(sd, ptr, strlen(ptr), 0,
				(struct sockaddr *)&sar, sizeof(sar));

			if (0 > ret) {
				perror("sendto() failed");
				exit(1);
			}
		}
	}

	(void) close(sd);

	return 0;
}

