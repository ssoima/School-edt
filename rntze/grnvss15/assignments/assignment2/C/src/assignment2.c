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

#include "ndisc.h"
#include "raw.h"
#include "hexdump.h"
#include "checksums.h"


/* Extracted from /usr/include/linux/if_ethernet.h (just for reference):
 * #define ETH_ALEN 6			 Octets in one ethernet addr
 *
 * Extracted from /usr/include/net/ethernet.h (just for reference):
 * struct ether_header
 * {
 *  u_int8_t  ether_dhost[ETH_ALEN];	destination eth addr
 *  u_int8_t  ether_shost[ETH_ALEN];	source ether addr
 *  u_int16_t ether_type;		packet type ID field
 * }
 */

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




/*====================================TODO===================================*/

/* First, declare struct useful for package dissection, i.e., a struct
 * containing * pointers to the various headers and header fields that will be
 * set by your * dissector (the function that tests whether or not a received
 * frame/packet is valid).
 */



/*
 * It is very useful to look into the files at /usr/include/netinet for
 * existing structs that may help to create these. Also have a look at
 * assignment1.c, e.g. 'struct wol'.
 */

struct neighbor_solicit_payload {
	uint8_t dummy[1];
} __attribute__((packed));

struct icmp6_neighbor_solicit {
	uint8_t dummy[1];
} __attribute__((packed));

struct neighbor_advertise_payload {
	uint8_t dummy[1];
} __attribute__((packed));

struct icmp6_neighbor_advertise {
	uint8_t dummy[1];
} __attribute__((packed));


/*
 * This function parses the ip address from a string into an network byte order
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
 * This function initializes an ethernet header
 *
 * @param hdr A pointer to the ethernet header
 * @param dst_mac A pointer to a buffer that contains the destination mac
 * @param src_mac A pointer to a buffer that contains the mac of this host
 * @param ethertype The ethernet type in host byte order
 */
void init_ether_header(struct ether_header *hdr, const uint8_t * dst_mac,
                       const uint8_t * src_mac, uint16_t ethertype)
{
	(void)hdr; (void)dst_mac; (void)src_mac; (void)ethertype;
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
void init_ip6_header(struct ipv6_hdr *hdr, struct in6_addr * dst_ip,
		     struct in6_addr * src_ip, uint8_t next,
		     uint16_t plen, uint8_t hoplimit, uint32_t label)
{
	(void)hdr; (void)dst_ip; (void)src_ip; (void) next; (void) plen;
	(void)hoplimit; (void)label;
}

/*
 * This function initializes the icmpv6 header and the neighbor discovery
 * payload
 *
 * @param payload A pointer to the buffer in which to initialize
 * @param dst_ip A pointer to a buffer that contains the destination ip
 * @param src_mac A pointer to a buffer that contains the mac of this host
 */
void init_icmp6_ndisc(struct neighbor_solicit_payload * payload,
		      const struct in6_addr * dst_ip,
		      const uint8_t * src_mac)
{
	(void)payload; (void)dst_ip; (void)src_mac;
}

/*
 * This function checks whether the ethernet header may be for a neighbor
 * advertisment which is sent as response to our neighbor discovery
 *
 * @param hdr A pointer to the header
 * @param mymac A pointer to a buffer which contains the mac of this host
 *
 * @return 0 on fail
 *	   1 on success
 */
uint8_t is_my_ether_header(struct ether_header * hdr, const uint8_t * mymac)
{
	(void)hdr; (void)mymac;
	return 0;
}

/*
 * This function checks whether the ip ethernet payload is an ipv6 header and if
 * it may be a neighbor advertisment sent in response to our neighbor discovery
 *
 * @param hdr A pointer to the ethernet payload
 * @param myip A pointer to a buffer containing the ip address of this host
 *
 * @return 0 on fail
 *	   1 on success
 */
uint8_t is_my_ip6_header(struct ipv6_hdr * hdr, const struct in6_addr * myip,
						const struct in6_addr * dst_ip)
{
	(void)hdr; (void)myip; (void)dst_ip;
	return 0;
}

/*
 * This function checks whether the ip payload is a neighbor advertisment sent
 * to us
 *
 * @param hdr A pointer to the icmp payload
 * @param dst_ip A pointer to a buffer containing the destination ip
 * @return 0 on fail
 *	   1 on success
 */
uint8_t is_my_neighbor_advertisment(struct nd_neighbor_advert * hdr,
						const struct in6_addr * dst_ip)
{
	(void)hdr; (void)dst_ip;
	return 0;
}

/*
 * This function checks if the frame received is a neighbor advertisment sent as
 * response to our neighbor discovery. If it is, it returns a pointer to the mac
 * address in the frame.
 * It calls the more specialized functions in succession to check their parts
 * and advances over optional ipv6 extension headers.
 *
 * @param buffer A pointer to the received frame
 * @param len The size of the received frame
 * @param mymac A pointer to a buffer which contains the mac of this host
 * @param myip A pointer to a buffer which contains the ip of this host
 * @param dst_ip A pointer to a buffer which contains the destination ip
 *
 * @return NULL on fail
 *	   A pointer to the destination mac address sent with the neighbor
 *	   advertisment
 */
uint8_t * retrieve_mac(uint8_t * buffer, ssize_t len, const uint8_t * mymac,
		       const struct in6_addr * myip,
		       const struct in6_addr * dst_ip)
{
	(void)buffer; (void)len; (void)mymac; (void)myip; (void)dst_ip;
	return NULL;
}

/*===========================================================================*/

void assignment2(int fd, const char * ipaddr, const int timeoutval)
{
	uint8_t recbuffer[1514];
	size_t length;
	struct icmp6_neighbor_solicit packet;
	struct in6_addr dip6;
	struct in6_addr sip6;
	ssize_t ret;
	unsigned timeout;
	uint8_t * mac;

	memset(&packet, 0, sizeof(packet));

	if( 0 > parse_ip(ipaddr, &dip6) ) {
		fprintf(stderr, "Wrong input format for destination address, "
			"input should be in format: a:b:c::1:2:3\n");
		return;
	}

/*====================================TODO===================================*/
	/*
	 * TODO:
	 * 1) Initialize the addresses required to build the packet and call
	 * the dedicated initialization functions.
	 * 2) Also send the packet over the network (call to libraw, see
	 * assignment1.c)
	 */

	length = 0;
/*===========================================================================*/

	hexdump(&packet, length);

	timeout = timeoutval*1000;

	while((ret = grnvs_read(fd, recbuffer, sizeof(recbuffer), &timeout))) {

		if((mac = retrieve_mac(recbuffer, ret, grnvs_get_hwaddr(fd),
				       &sip6, &dip6))) {
			break;
		}
	}
	if(ret == 0) {
		/*
		 * This should go to stdout because the tester uses this to
		 * determine a graceful shutdown. Do NOT change this
		 */
		fprintf(stdout, "Message timed out\n");
		return;
	}

/*====================================TODO===================================*/
	/*
	 *TODO: Print the retrived mac address.
	 * The format MUST strictly adhere to the following rule:
	 *
	 * <IP-Address> is at <mac>
	 *
	 * The IP-address MAY be shortened, the mac MUST be 2 characters for
	 * each byte. The result MUST be printed on stdout. Don't forget the
	 * newline!
	 *
	 * Example output:
	 * ::1 is at 01:02:03:04:05:06
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

	if ( (sock = grnvs_open(args.interface, SOCK_RAW)) < 0 ) {
		fprintf(stderr, "grnvs_open() failed: %s\n", strerror(errno));
		return -1;
	}

	assignment2(sock, args.dst, args.timeout);

	grnvs_close(sock);

	return 0;
}
