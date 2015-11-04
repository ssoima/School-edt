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
#include <netdb.h>
#include <signal.h>

#define MAX(a, b) ((a) < (b) ? (b) : (a))
#define MIN(a, b) ((a) < (b) ? (a) : (b))

#define BUFFLEN    1024

static int run;

static void sighandler(int signo)
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

static void
printHelp(const char *name)
{
	fprintf(stderr,"Usage: %s <server IP> <server port>\n\n",name);
}

int
main(int argc, char **argv)
{
	struct sockaddr_in sa_srv;
	struct hostent *h;
	int sd, len, maxfd, sa_srv_port;
	char buffer[BUFFLEN];
	char *s;
	fd_set rfds, rfd;

	/* Mask the infamous SIGPIPE */
	signal(SIGPIPE, SIG_IGN);
	signal(SIGINT, sighandler);
	signal(SIGTERM, sighandler);

	/* Check arguments and printHelp/exit when something is wrong */
	if (argc != 3) {
		printHelp(argv[0]);
		exit(1);
	}

	/* Initialize sockaddr structure */
	memset(&sa_srv, 0, sizeof(sa_srv));
	sa_srv_port = atoi(argv[2]);

	/* Resolve server name (FQDN to IP address) */
	if (NULL == (h = gethostbyname2(argv[1], AF_INET))) {
		fprintf(stderr, "unable to resolve hostname %s\n", argv[1]);
		exit(1);
	}

	/* Prepare sockaddr structure used to connect to our server */
	memcpy(&sa_srv.sin_addr.s_addr, h->h_addr_list[0], h->h_length);
	sa_srv.sin_family	= AF_INET;
	sa_srv.sin_port		= htons(sa_srv_port);

	if (0 > (sd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP))) {
		perror("socket() failed");
		exit(1);
	}
	if (0 > connect(sd, (struct sockaddr *)&sa_srv, sizeof(sa_srv))) {
		perror("connect() failed");
		exit(1);
	}

	/* Prepare the fd_set */
	FD_ZERO(&rfds);
	FD_SET(STDIN_FILENO, &rfds);
	FD_SET(sd, &rfds);
	maxfd = MAX(STDIN_FILENO, sd);

	/* Enter the event loop */
	run = 1;
	while (run) {
		rfd = rfds;

		/* Wait for something interesting */
		if (0 > select(maxfd+1, &rfd, NULL, NULL, NULL)) {
			if (errno == EINTR)
				continue;
			perror("select() failed");
			exit(1);
		}
	
		if (FD_ISSET(STDIN_FILENO, &rfd)) {
			/* Read from STDIN */
			memset(buffer, 0, sizeof(buffer));
			if (NULL == (s=fgets(buffer,sizeof(buffer)-1,stdin)))
				continue;
			len = send(sd, buffer, strlen(buffer)+1, 0);
			if (0 > len) {
                		perror("send() failed");
				exit(1);
			}
		}

	        if (FD_ISSET(sd, &rfd)) {
			/* Read from socket */
			do {
				memset(buffer, 0, sizeof(buffer));
				len = recv(sd, buffer, BUFFLEN-1, MSG_DONTWAIT);
				if (len > 0)
					fprintf(stdout, "%s", buffer);
			} while (len > 0);
		
			if (0 > len) {
				if (errno == EINTR || errno == EWOULDBLOCK)
					continue;
				perror("recv() failed");
				exit(1);
			}
			
			if (0 == len) {
				fprintf(stdout, "Looks that the server has "
					"closed the connection\n");
				run = 0;
			}
		}
	}

	close(sd);

	return 0;
}

