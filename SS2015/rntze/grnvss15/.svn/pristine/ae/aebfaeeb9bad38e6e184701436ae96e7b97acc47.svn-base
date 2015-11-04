#include <netinet/ether.h>
#include <net/ethernet.h>
#include <stdio.h>
#include <arpa/inet.h>
#include <string.h>
#include <errno.h>
#include <sys/types.h>
#include <stdlib.h>
#include <netinet/ip6.h>
#include <netinet/icmp6.h>
#include <asm/byteorder.h>
#include <unistd.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <sys/socket.h>

#include "asciiclient.h"


/*
 * This function parses the ip from an address string into a machine readable
 * format.
 *
 * @param dst_ip A pointer to a struct in6_addr where the ip will be written to
 * @param ipaddr The ip address as null-terminated string
 *
 * @return  0 on success
 *         -1 on error
 */
int parse_ip(const char *ipaddr, struct in6_addr *dst_ip)
{
        return inet_pton(AF_INET6, ipaddr, dst_ip) - 1;
}

/*
 * Use this function to set a timeout on a socket.
 * Once a timeout is set, the socket operations will time out after the
 * specified time (given in seconds)
 *
 * The timeout will reset between function calls.
 */
int set_socket_options(int socket, int timeout)
{
        struct timeval time;
        int eins = 1;
        time.tv_sec = timeout;
        time.tv_usec = 0;

        if (setsockopt(socket, SOL_SOCKET, SO_RCVTIMEO, (char *)&time,
                                                        sizeof(time)) < 0) {
                fprintf(stderr, "Failed to set recv timeout: %s\n",
                                strerror(errno));
                return -1;
        }

        if (setsockopt(socket, SOL_SOCKET, SO_SNDTIMEO, (char *)&time,
                                                        sizeof(time)) < 0) {
                fprintf(stderr, "Failed to set send timeout: %s\n",
                                strerror(errno));
                return -1;
        }

        /*
         * This sets reuseaddr for the server socket, so we can create a new
         * listening server before the old one completelly dies (in case of a
         * crash or some tcp FIN_WAIT
         */
        if(setsockopt(socket, SOL_SOCKET, SO_REUSEADDR, &eins, sizeof(eins))<0){
                fprintf(stderr, "setsockopt() failed: %s\n", strerror(errno));
                return -1;
        }
        return 0;
}


/*====================================TODO===================================*/
/**
 * Write a netstring to a socket.
 * The input data must first be encoded as netstring:
 * <length of data in decimal>":"<data>","
 *
 * \param sock the socket to send the data on
 * \param src A pointer to the data
 * \param length The length of the data
 * \return 0 on success
 *         -1 on fail
 */
int send_netstring(int sock, const uint8_t *src, size_t length)
{
        (void) sock; (void) src; (void) length;
        return -1;
}


/**
 * Read a netstring from a socket.
 * The netstring may be fragmented, so this function might need to call
 * read() multiple times.
 * Since TCP is stream oriented, there may also be more than one
 * netstring buffered. This function must NOT read into the next
 * netstring, so it can be read later without problems.
 * This function must validate the netstring and strip the length
 * (including the ':') and the trailing ','.
 * The function returns the length of the content or -1 on error.
 *
 * \param sock The socket to read the data from
 * \param buf A pointer to a pointer where to write the
 *            pointer to the newly allocated memory for the payload of
 *            the netstring
 * \return The size of the netstring
 */
int receive_netstring(int sock, uint8_t **buf)
{
        (void) sock; (void) buf;
        return -1;
}

/**
 * This function creates a connection to a server by opening a socket and
 * calling connect().
 *
 * \param dstip The IP of the server
 * \param port The port to connect to
 *
 * \return The socket on success
 *         -1 if an error occurs
 */
int start_connection(const struct in6_addr *dstip, in_port_t port)
{
        (void) dstip; (void) port;
        return -1;
}

/**
 * This function creates a socket that listens on the given port,
 * ready to accept connections.
 * Setting a timeout with set_socket_options of 5s is recommended.
 * NOTE: it is important to call set_socket_options before you call bind/listen
 *
 * \param port The port to listen on
 * \return The new socket
 */
int start_listen(in_port_t port)
{
        (void) port;
        return -1;
}

/**
 * This function should accepts a new connection on the server socket.
 *
 * \param sock The socket to use for accepting
 *
 * \return The socket connected to the new client
 *         -1 if an error occurs
 */
int accept_server(int sock)
{
        (void) sock;
        return -1;
}


/**
 * This function should request for a message to be fetched by the server.
 *
 * \param sock The control connection socket
 * \param nick The nickname to use
 * \param port The port to request for the data connection
 * \param token A pointer to a pointer where to write the pointer
 *              to newly allocated memory for the server token
 * \return The size of the token
 *         -1 on fail
 */
ssize_t request_transfer(int sock, const char *nick, const char *port,
                         uint8_t **token)
{
        (void) sock; (void) nick; (void) port; (void) token;
        return -1;
}


/**
 * This function transfers the message in the data connection.
 *
 * \param sock  The data connection socket
 * \param msg   The message to send to the server
 * \param nick  The nick as sent in the control connection
 * \param token The token sent by the server in the first phase of the
 *              protocol
 * \param length The length of the token
 * \param dtoken A pointer to a pointer where to write the pointer
 *               to the newly allocated memory for the data token
 * \return The size of the new token
 *         -1 on fail
 */
ssize_t send_message(int sock, const char *msg, const char *nick,
                     const uint8_t *token, ssize_t length,
                     uint8_t **dtoken)
{
        (void) sock; (void) msg; (void) nick; (void) token; (void) length;
        (void) dtoken;
        return -1;
}


/**
 * This function commits the message after it has been transferred in
 * the data connection.
 *
 * \param sock The control connection socket
 * \param dtoken The data token returned by the server after the message
 *               has been transferred.
 * \param size The length of the data token
 * \param msglen The length of the msg sent to the server
 * \return 0 on succes
 *         -1 on fail
 */
int commit_message(int sock, uint8_t *dtoken, ssize_t size, size_t msglen)
{
        (void) sock; (void) dtoken; (void) size; (void) msglen;
        return -1;
}

/*===========================================================================*/


/**
 * This is the entry point for the asciiclient.
 * It uses request_transfer, send_message and commit_message to post a
 * unicode message to the server.
 *
 * \param ipaddr The IP address in ASCII representation. IPv6 has to be supported,
 *               IPv4 is optional
 * \param port The server port to connect to
 * \param nick The nickname to use in the protocol
 * \param msg The message to send
 */
void assignment4(const char *ipaddr, in_port_t port, char *nick, char *msg)
{
        struct in6_addr dstip;

        (void) ipaddr; (void) port; (void) nick; (void) msg; (void) dstip;

        if( 0 > parse_ip(ipaddr, &dstip) ) {
                fprintf(stderr, "Wrong input format for destination address, "
                        "input should be in format: a:b:c::1:2:3\n");
                return;
        }

/*====================================TODO===================================*/
/*===========================================================================*/
}

int main(int argc, char ** argv)
{
        struct arguments args;
        int fd;
        char buffer[4000];

        if ( parse_args(&args, argc, argv) < 0 ) {
                fprintf(stderr, "Failed to parse arguments, call with "
                        "--help for more information\n");
                return -1;
        }

        if(args.file) {
                args.msg = buffer;
                if((fd = open(args.file, O_RDONLY)) < 0) {
                        fprintf(stderr, "Could not open file: %s - %s\n",
                                args.file, strerror(errno));
                        return -1;
                }
                if(read(fd, buffer, sizeof(buffer)-1) < 0) {
                        fprintf(stderr, "Could not read file: %s\n",
                                strerror(errno));
                        close(fd);
                        return -1;
                }
                close(fd);
        }

        setbuf(stdout, NULL);

        assignment4(args.dst, args.port, args.nick, args.msg);

        return 0;
}
