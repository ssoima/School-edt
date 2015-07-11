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

#include "traceroute.h"
#include "raw.h"
#include "hexdump.h"
#include "checksums.h"

/* Extracted from /usr/include/netinet/in.h (just for reference):
 * struct in6_addr
 *   {
 *     union
 *       {
 * 	uint8_t	__u6_addr8[16];
 * #ifdef __USE_MISC
 * 	uint16_t __u6_addr16[8];
 * 	uint32_t __u6_addr32[4];
 * #endif
 *       } __in6_u;
 * #define s6_addr		__in6_u.__u6_addr8
 * #ifdef __USE_MISC
 * # define s6_addr16		__in6_u.__u6_addr16
 * # define s6_addr32		__in6_u.__u6_addr32
 * #endif
 *   };
 *
 * "hdr->s6_addr" will  work as access.
 */

/* Extracted from /usr/include/netinet/icmp6.h (just for reference):
 * struct icmp6_hdr
 * {
 *   uint8_t     icmp6_type;   type field
 *   uint8_t     icmp6_code;   code field
 *   uint16_t    icmp6_cksum;  checksum field
 *   union
 *     {
 *       uint32_t  icmp6_un_data32[1]; type-specific field
 *       uint16_t  icmp6_un_data16[2]; type-specific field
 *       uint8_t   icmp6_un_data8[4];  type-specific field
 *     } icmp6_dataun;
 * };
 */



/*
 * We do not use the kernel's definition of the IPv6 header (struct ipv6hdr)
 * because the definition there is slightly different from what we would expect
 * (the problem is the 20bit flow label - 20bit is brain-damaged).
 *
 * Instead, we provide you struct that directly maps to the RFCs and lecture
 * slides below.
 */

struct ipv6_hdr {
#if defined(__LITTLE_ENDIAN_BITFIELD)
	uint32_t tc1:4, version:4, flow_label1:4, tc2:4, flow_label2:16;
#elif defined(__BIG_ENDIAN_BITFIELD)
	uint32_t version:4, tc1:4, tc2:4, flow_label1:4, flow_label2:16;
#else
#error "You did something wrong"
#endif
	uint16_t plen;
	uint8_t nxt;
	uint8_t hlim;
	struct in6_addr src;
	struct in6_addr dst;
} __attribute__((packed));


enum packet_types {
	packet_reply = 1,
	packet_exceed,
	packet_unreach
};


/*====================================TODO===================================*/

/* First, declare structs useful for package dissection, i.e., a struct
 * containing * pointers to the various headers and header fields that will be
 * set by your * dissector (the function that tests whether or not a received
 * frame/packet is valid).
 */


/*
 * A struct representing the echo request ip payload
 */
struct echo_request {
	uint8_t dummy[1];
} __attribute__((packed));

/*
 * A struct representing the complete echo request packet
 */
struct echo_packet {
	uint8_t dummy[1];
} __attribute__((packed));

/*
 * A struct representing a time exceeded packet
 */
struct ttl_packet {
	uint8_t dummy[1];
} __attribute__((packed));

/*
 * A struct representing a destination unreachable packet
 */
struct unreachable_packet {
	uint8_t dummy[1];
} __attribute__((packed));

/*
 * This function parses the ip address from a string into a network byte order
 * representation
 *
 * @param dst_ip A pointer to a struct in6_addr where the ip will be written to
 * @param ipaddr The ip address as null-terminated string
 *
 * @return  0 on success
 *         -1 on error
 */
int parse_ip(const char *ipaddr, struct in6_addr *dst_ip)
{
	(void)ipaddr; (void)dst_ip;

	return 0;
}


/*
 * This function initializes an ipv6 header
 * This encompasses the version and everything given as parameter
 *
 * @param hdr A pointer to the ip header
 * @param dst_ip A pointer to a buffer that contains the destination ip
 * @param src_ip A pointer to a buffer that contains the ip of this host
 * @param next The next hop field for this header
 * @param plen The size of the ip payload in host byte order
 * @param hoplimit The hoplimit for the ip packet
 * @param label The label for the ip packet:
 * 	  The last 20 bit of the label should be used!
 * 	  The label should be in proper byte order already
 */
void init_ip6_header(struct ipv6_hdr *hdr, const struct in6_addr *dst_ip,
		     const struct in6_addr *src_ip, uint8_t next,
		     uint16_t plen, uint8_t hoplimit, uint32_t label)
{
	(void)hdr; (void)dst_ip; (void)src_ip; (void) next; (void) plen;
	(void)hoplimit; (void)label;


}

/*
 * This function initializes the icmpv6 header and the echo request
 *
 * @param payload A pointer to the buffer in which to initialize
 * @param id The id that should be used for this echo request
 * @param seq The sequence number that should be used for this echo request
 */
void init_echo_request(struct echo_request * req,
		       uint16_t id, uint16_t seq)
{
	(void) req; (void) id; (void) seq;
}

/*
 * The ipv6 header and if it may be a reaction to our echo request
 *
 * @param hdr A pointer to the packet
 * @param myip A pointer to a buffer containing the ip address of this host
 *
 * @return 0 on fail
 *	   1 on success
 */
uint8_t is_my_ip6_header(struct ipv6_hdr * hdr, const struct in6_addr * myip)
{
	(void)hdr; (void)myip;

	return 1;
}

/*
 * This function checks whether the IP payload is an echo reply sent in
 * response to our echo request
 *
 * @param hdr A pointer to the icmp payload
 * @param id The id that was used in the echo request
 * @param seq The sequence number that was used for the echo request
 * @return 0 on fail
 *	   1 on success
 */
uint8_t is_my_echo_reply(struct echo_request *hdr, uint16_t id, uint16_t seq,
			 const struct in6_addr *dst_ip)
{
	(void)hdr; (void)id; (void) seq; (void) dst_ip;
	return 1;
}

/*
 * This function checks if the received packet is a destination unreachable
 * icmp message sent in response to our echo request
 *
 * @param packet A pointer to the buffer where the received packet is stored
 * @param len The length of the received payload
 * @param sent A pointer to the original echo request sent
 * @param sentlen The length of the original echo request
 * @return 0 on fail
 *	   1 on success
 */
uint8_t is_my_unreachable(struct unreachable_packet *packet, size_t len,
			  const struct echo_packet *sent, size_t sentlen)
{
	(void) packet; (void) sent; (void) sentlen; (void) len;

	return 1;
}

/*
 * This function checks if the received packet is a time exceeded
 * icmp message sent in response to our echo request
 *
 * @param buffer A pointer to the buffer where the received packet is stored
 * @param len The length of the received payload
 * @param sent A pointer to the original echo request sent
 * @param sentlen The length of the original echo request
 * @return 0 on fail
 *	   1 on success
 */
uint8_t is_my_time_exceed(struct ttl_packet * buffer, size_t len,
			  const struct echo_packet *sent, size_t sentlen)
{
	(void) buffer; (void) len; (void) sent; (void) sentlen;

	return 1;
}


/*
 * This function checks whether the received packet is for us
 * It calls the more specialized functions in succession to check their parts
 * and advances over optional ipv6 extension headers.
 *
 * @param buffer A pointer to the received frame
 * @param len The size of the received frame
 * @param id The id, which we would expect in our packet
 * @param seq The sequence nr, which we would expect in our packet
 * @param myip A pointer to a buffer which contains the ip of this host
 * @param dstip A pointer to a buffer which contains the destination ip
 * @param sent A pointer to the original echo request sent
 * @param sentlen The length of the original echo request
 *
 * @return 0 If the packet was not destined for us
 *	   The fitting value from the packet_types enum
 */
int get_hop_ip(uint8_t * buffer, ssize_t len, uint16_t id, uint16_t seq,
	       const struct in6_addr * myip, const struct in6_addr *dstip,
	       const struct echo_packet *sent, size_t sentlen,
	       struct in6_addr **hopip)
{
	(void)buffer; (void)len; (void)myip; (void)dstip; (void) id;
	(void) seq; (void) sent; (void) sentlen; (void) hopip;


	return 0;
}



/*
 * This function sends out an echo request packet to the network and reads from
 * the network until either a response to the packet is found or the timeout
 * expires
 *
 * It also converts the ipaddress, read from the network, into a human readable
 * representation.
 *
 * @param packet A pointer to the packet to send
 * @param length The length of the packet to send
 * @param srcip The ip of this host
 * @param dstip The ip of the echo request destination host
 * @param id The id used in the echo request
 * @param seq The sequence number used in the echo request
 * @param hopstr A pointer to a buffer where the human readable string should be
 *               written to
 * @param maxlen The size of the buffer
 * @param fd The file descriptor for the read/write operations
 * @param timeoutval The timeout in seconds
 *
 * @return 0 If the request timed out
 *	   The fitting value from the packet_types enum
 */
int run_attempt(struct echo_packet *packet, size_t length,
		const struct in6_addr *srcip, const struct in6_addr *dstip,
		uint16_t id, uint16_t seq, char *hopstr, size_t maxlen, int fd,
		const int timeoutval)
{
	uint8_t recbuffer[1514];
	struct in6_addr *hopip;
	unsigned timeout;
	(void) packet; (void) length; (void) srcip; (void) dstip; (void) id;
	(void) seq; (void) hopstr; (void) maxlen; (void) fd; (void) timeoutval;
	(void) recbuffer; (void) hopip; (void) timeout;

	return 0;
}

/*===========================================================================*/

void assignment3(int fd, const char *ipaddr, int timeoutval, int attempts,
		 int hoplimit)
{
	char ipname[INET6_ADDRSTRLEN];
	struct in6_addr dstip;
	struct in6_addr srcip;
	struct echo_packet packet;
	size_t length;
	int seq;

	(void) dstip; (void) srcip; (void) packet; (void) length; (void) seq;
	(void) ipname; (void) fd; (void) timeoutval; (void) attempts;
	(void) hoplimit;

	if( 0 > parse_ip(ipaddr, &dstip) ) {
		fprintf(stderr, "Wrong input format for destination address, "
			"input should be in format: a:b:c::1:2:3\n");
		return;
	}
/*====================================TODO===================================*/
	/*
	 * TODO:
	 * 1) Initialize the addresses required to build the packet.
	 * 2) Loop over hoplimit and attempts
	 * 3) Build and send a packet for each iteration
	 * 4) Print the hops found in the specified format
	 */

/*===========================================================================*/
}

int main(int argc, char ** argv)
{
	struct arguments args;
	int sock;

	if ( parse_args(&args, argc, argv) < 0 ) {
		fprintf(stderr, "Failed to parse arguments, call with "
			"--help for more information\n");
		return -1;
	}

	if ( (sock = grnvs_open(args.interface, SOCK_DGRAM)) < 0 ) {
		fprintf(stderr, "grnvs_open() failed: %s\n", strerror(errno));
		return -1;
	}

	setbuf(stdout, NULL);

	assignment3(sock, args.dst, args.timeout, args.attempts, args.hoplimit);

	grnvs_close(sock);

	return 0;
}
