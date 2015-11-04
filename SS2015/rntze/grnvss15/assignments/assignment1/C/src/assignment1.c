#include <netinet/ether.h>
#include <net/ethernet.h>
#include <stdio.h>
#include <arpa/inet.h>
#include <string.h>
#include <errno.h>
#include <sys/types.h>
#include <stdlib.h>

#include "wol.h"
#include "raw.h"
#include "hexdump.h"


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

struct wol_payload {
	uint8_t sync[6];
	uint8_t target[96];
} __attribute__((packed)); // <- do not remove that

struct magic_packet {
	struct ether_header hdr;
	struct wol_payload wol;
} __attribute__((packed)); // <- do not remove that


/*====================================TODO===================================*/
int parse_mac(const char *hwaddr, uint8_t *dst_mac)
{
	(void)hwaddr; (void)dst_mac;

	//TODO: Parse the MAC address (string) pointed to by hwaddr and store
	//the result in dst. Return 0 on success and -1 on error.
	
	return 0;
}

void init_ether_header(struct ether_header *hdr, const uint8_t *dst_mac,
				const uint8_t *src_mac, uint16_t ethertype)
{
	(void)hdr; (void)dst_mac; (void)src_mac; (void)ethertype;

	//TODO: Initialize the ether header here.
}

void init_wol_payload(struct wol_payload *wol, const uint8_t *dst_mac)
{
	(void)wol, (void)dst_mac;

	//TODO: Initialize the WoL payload here.
}
/*===========================================================================*/

void assignment1(int fd, const char * hwaddr)
{
	size_t length;
	struct magic_packet packet;
	uint8_t dmac[ETH_ALEN];

	memset(&packet, 0, sizeof(packet));

	if( 0 > parse_mac(hwaddr, dmac) ) {
		fprintf(stderr, "Wrong input format for destination address, "
			"input should be in format: xx:xx:xx:xx:xx:xx\n");
		return;
	}

/*====================================TODO===================================*/
	//TODO: write ether-header
	// -> use init_ether_header()
	// -> grnvs_get_hwaddr() defined in raw.h returns your source address
	// -> determine the correct ethertype and take care of endianess

	//TODO: write WoL payload
	// -> use init_wol_payload()

	//TODO: Determine the size of the WoL packet and set it here.
	length = 0;
	hexdump(&packet, length);
/*===========================================================================*/

	if(grnvs_write(fd, &packet, length) < 0 ) {
		fprintf(stderr, "grnvs_write() failed: %s\n", strerror(errno));
	}
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

	assignment1(sock, args.dst);

	grnvs_close(sock);

	return 0;
}
