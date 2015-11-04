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

#include "list.h"

#define MAX(a, b) ((a) < (b) ? (b) : (a))
#define MIN(a, b) ((a) < (b) ? (a) : (b))

#define BUFFLEN    1024

struct client {
	struct list_head list;
	struct sockaddr_in sa;
	int sd;
	socklen_t slen;
};

static LIST_HEAD(cl);
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

static void
printHelp(const char *name)
{
	fprintf(stderr,"Usage: %s <server port>\n\n",name);
}

int
main(int argc, char **argv)
{
	struct sockaddr_in sa;
	struct client *cur;
	int sd, len, maxfd, port;
	char from[128];
	char msg[1024];
	char *s;
	fd_set rfds, rfd;

	/* Mask the infamous SIGPIPE */
	signal(SIGPIPE, SIG_IGN);
	signal(SIGINT, sighandler);
	signal(SIGTERM, sighandler);

	/* Check arguments and printHelp/exit when something is wrong */
	if (argc != 2) {
		printHelp(argv[0]);
		exit(1);
	}

	/* Initialize sockaddr structure */
	memset(&sa, 0, sizeof(sa));
	port = atoi(argv[1]);

	/* Prepare sockaddr structure used to connect to our server */
	sa.sin_addr.s_addr	= INADDR_ANY;
	sa.sin_family		= AF_INET;
	sa.sin_port		= htons(port);

	if (0 > (sd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP))) {
		perror("socket() failed");
		exit(1);
	}
	if (0 > bind(sd, (struct sockaddr *)&sa, sizeof(sa))) {
		perror("bind() failed");
		exit(1);
	}
	if (0 > listen(sd, 16)) {
		perror("listen() failed");
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
		memset(from, 0, sizeof(from));
		memset(msg, 0, sizeof(msg));
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
			if (NULL == (s=fgets(msg,sizeof(msg)-1,stdin)))
				continue;
			snprintf(from, sizeof(from), "SERVER>> ");
		}
	        else if (FD_ISSET(sd, &rfd)) {
			/* New client */
			cur = malloc(sizeof(*cur));
			memset(cur, 0, sizeof(*cur));

			cur->slen = sizeof(cur->sa);
			cur->sd = accept(sd, (struct sockaddr *)&cur->sa,
				&cur->slen);

			if (0 > cur->sd) {
				perror("accept() failed");
				free(cur);
				continue;
			}

			list_add(&cur->list, &cl);
			FD_SET(cur->sd, &rfds);
			maxfd = MAX(maxfd, cur->sd);

			snprintf(from, sizeof(from), "SERVER>> ");
			snprintf(msg, sizeof(msg), "new client connected from %s:%d\n",
				inet_ntoa(cur->sa.sin_addr),
				ntohs(cur->sa.sin_port));
		}
		else {
			list_for_each_entry(cur, &cl, list) {
				if (FD_ISSET(cur->sd, &rfd))
					break;
			}

			if (&cur->list == &cl)
				continue;

			len = recv(cur->sd, msg, sizeof(msg),
					MSG_DONTWAIT);
			
			if (0 > len) {
				if (errno == EINTR || errno == EWOULDBLOCK)
					continue;
			}
			if (0 >= len) {
				/* client disconnected */
				FD_CLR(cur->sd, &rfds);

				snprintf(from, sizeof(from), "SERVER>> ");
				snprintf(msg, sizeof(msg), "client disconnected %s:%d\n",
				inet_ntoa(cur->sa.sin_addr),
				ntohs(cur->sa.sin_port));

				list_del(&cur->list);
				free(cur);
			}
			else {	
				snprintf(from, sizeof(from), "%s:%d>> ",
					inet_ntoa(cur->sa.sin_addr),
					ntohs(cur->sa.sin_port));
			}
		}

		s = sanitize(msg, sizeof(msg));
		if (strlen(s) == 1)
			continue;

		fprintf(stdout, "%s", from);
		fprintf(stdout, "%s", s);

		list_for_each_entry(cur, &cl, list) {
			(void) send(cur->sd, from, strlen(from), 0);
			(void) send(cur->sd, s, strlen(s), 0);
		}
	}

	close(sd);

	return 0;
}

