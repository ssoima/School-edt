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
	struct icmp6_hdr  nd_ns_hdr;
    struct in6_addr   nd_ns_target;
    struct nd_opt_hdr nd_opt_hdr;
	uint8_t * src_mac;
} __attribute__((packed));

struct icmp6_neighbor_solicit {
	struct ether_header eth;
	struct ipv6_hdr ipv6_hdr;
	struct neighbor_solicit_payload neighbor_solicit_payload;
} __attribute__((packed));

struct neighbor_advertise_payload {
	struct icmp6_hdr  nd_na_hdr;
    struct in6_addr   nd_na_target;
    struct nd_opt_hdr nd_opt_hdr;
	uint8_t * target_mac;
} __attribute__((packed));

struct icmp6_neighbor_advertise {
	struct ether_header eth;
	struct ipv6_hdr ipv6_hdr;
	struct neighbor_advertise_payload neighbor_advertise_payload;
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

	int i;
	int rest=0;
	int fourdigitct=0;
	int zweipktct=0;

	//Check if ip is valid	
	for(i=0;i<strlen(ipaddr);i++){
		if(ipaddr[i]==':')
		{
			fourdigitct=0;
			if (ipaddr[i+1]==':')
				if (rest==0)
				{
					rest=1;
					i++;
				}
				else if (rest==1)
					return -1;
			zweipktct++;
		}
		else 
		{
			if (fourdigitct>3)
				return -1;
			if(!isxdigit(ipaddr[i]))
				return -1; 
			fourdigitct++;
		}
		if (zweipktct>7)
			return -1;
	}
	
	//store ip
	int ct,total=0,j;

	for(i=0;i<strlen(ipaddr);i++){
		ct=0;
		if ( ipaddr[i]==':')
		{
			for(j=0;j<4*(7-zweipktct);j++)
			dst_ip->s6_addr[total++]=0;
		}
		else
		{
			while(ipaddr[i+ct]!=':' && total+ct<8*4)
			{
				ct++;
			}
			for(j=0;j<4-ct;j++)
				dst_ip->s6_addr[total++]=0;
			for(j=0;j<ct;j++)
				dst_ip->s6_addr[total++]=strtol(strndup(ipaddr+i+j,1), NULL, 16);
			i+=ct;
				
		}
	}
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
	memcpy(&hdr->ether_dhost,dst_mac,ETH_ALEN);
	memcpy(&hdr->ether_shost,src_mac,ETH_ALEN);
	hdr->ether_type = htons(ethertype);
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

	memcpy(&hdr->dst.s6_addr, dst_ip, 16);
	memcpy(&hdr->src.s6_addr, src_ip, 16);
	memcpy(&hdr->nxt, next, sizeof(next));
	memcpy(&hdr->plen, plen, sizeof(plen));
	memcpy(&hdr->hlim,hoplimit, sizeof(hoplimit));	
	memcpy(hdr,label, sizeof(label));
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
	memcpy(&payload->nd_ns_target, dst_ip, sizeof(dst_ip));
	memcpy(&payload->src_mac, src_mac, sizeof(src_mac));
	payload->nd_ns_hdr.icmp6_type=0x87;
	payload->nd_ns_hdr.icmp6_code=0x00;
	payload->nd_opt_hdr.nd_opt_type=0x01;
	payload->nd_opt_hdr.nd_opt_type=0x01;


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
	//int compare_mymac=memcmp(hdr->ether_dhost, mymac->s6_addr, sizeof(hdr->ether_dhost));

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
	int i;
	for(i=0;i<ETH_ALEN;i++)
		if(hdr->ether_dhost[i]!=mymac[i])
			return 0;
	//if(compare_mymac==0
		if (hdr->ether_type==0xDD86)
		return 1;
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
	if(hdr->version!=0x6
		&& hdr->tc1!=0x00
		&& hdr->tc2!=0x00
		&& hdr->hlim!=255
		&& hdr->flow_label1!=0
		&& hdr->flow_label2!=0
		&& hdr->nxt!=0x3a
		&& hdr->dst.s6_addr!=myip->s6_addr
		&& hdr->src.s6_addr!=dst_ip->s6_addr)  /*TBC*/
		return 0;
	return 1;

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
	if(hdr->nd_na_target.s6_addr==dst_ip->s6_addr     /*TBC*/
		&& hdr->nd_na_hdr.icmp6_type==0x88 
		&& hdr->nd_na_hdr.icmp6_code==0x00)
		return 1;
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
    struct ether_header *h = buffer;
    if (is_my_ether_header(h,mymac))
    {
    	/*struct ether_header * hdr, const uint8_t * mymac*/
    }
	return NULL;
}
  
int parse_mac(const char *hwaddr, uint8_t *dst_mac)
{
	(void)hwaddr; (void)dst_mac;

	//TODO: Parse the MAC address (string) pointed to by hwaddr and store
	//the result in dst. Return 0 on success and -1 on error.
	//check correct Mac
	int i;
	if (strlen(hwaddr)!=17)
		return -1;
	for(i=0;i<strlen(hwaddr);i++){
		if(i!=0 && i%3==2 && hwaddr[i]!=':')
			return -1; 
		if(i%3!=2 && !isxdigit(hwaddr[i])) 
			return -1;
	}

	//Create u_int mac
	for(i=0;i<ETH_ALEN;i++)
		dst_mac[i]=(uint8_t)atoi(strndup(hwaddr+(i*3),2)); 
	return 0;
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
	uint8_t *dst_mac;
	parse_mac("ff:ff:ff:ff:ff:ff",dst_mac);
	init_icmp6_ndisc(&(packet.neighbor_solicit_payload), &dip6,grnvs_get_hwaddr(fd));
	init_ip6_header(&(packet.ipv6_hdr), &dip6, grnvs_get_ip6addr(fd), 0x3a, sizeof(packet.neighbor_solicit_payload), 255, 0);
	init_ether_header(&(packet.eth), dst_mac ,grnvs_get_hwaddr(fd),htons(0x86DD));

	length = sizeof(packet);
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
	 char  mac_char[17];
	 int i=0,ct=0;
	 for(i=0;i<17;i++)
	 	if(i%3!=2)
	 		mac_char[i]=mac[i-ct];
	 	else{
	 		mac_char[i]=':';
	 		ct++;
	 	}

	 struct icmp6_neighbor_advertise *advertise_packege = recbuffer;
	 char ip_char[32+7];
	 for (i=0;i<32+7;i++)
	 	if(i%5!=4)
	 		mac_char[i]=mac[i-ct];
	 	else{
	 		mac_char[i]=':';
	 		ct++;
	 	}


/*
	 advertise_packege->ipv6_hdr.src.s6_addr.;

struct neighbor_advertise_payload {
	struct icmp6_hdr  nd_na_hdr;
    struct in6_addr   nd_na_target;
    struct nd_opt_hdr nd_opt_hdr;
	uint8_t * target_mac;
} __attribute__((packed));

struct icmp6_neighbor_advertise {
	struct ether_header eth;
	struct ipv6_hdr ipv6_hdr;
	struct neighbor_advertise_payload neighbor_advertise_payload;
} __attribute__((packed));   */

	 fprintf( stdout, "%s is at %s\n",ip_char,mac_char);

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

/* Parse ip
#include <stdio.h>
#include <string.h>

int main(void) {
	if(parse_ip("2a00:4700:0:3::f9f4:fa12")==0)
		printf("");
	else
		printf("bad ip");
	char x[10]="124f1234";
	printf("\n bla %d",strtol(strndup(x+3,1), NULL, 16));
	return 0;
}

int parse_ip(const char *ipaddr)
{
	(void)ipaddr;

	int i;
	int rest=0;
	int fourdigitct=0;
	int zweipktct=0;
	//if (strlen(ipaddr)!=8*4+7)
	//	return -1;
	
	for(i=0;i<strlen(ipaddr);i++){
		if(ipaddr[i]==':')
		{
			fourdigitct=0;
			if (ipaddr[i+1]==':')
				if (rest==0)
				{
					rest=1;
					i++;
				}
				else if (rest==1)
					return -1;
			zweipktct++;
		}
		else 
		{
			if (fourdigitct>3)
				return -1;
			if(!isxdigit(ipaddr[i]))
				return -1; 
			fourdigitct++;
		}
		if (zweipktct>7)
			return -1;
	}
	
	int x[8*4+7],ct,total=0,j;

	for(i=0;i<strlen(ipaddr);i++){
		ct=0;
		if ( ipaddr[i]==':')
		{
			for(j=0;j<4*(7-zweipktct);j++)
			x[total++]=0;
		}
		else
		{
			while(ipaddr[i+ct]!=':' && total+ct<8*4)
			{
				ct++;
			}
			for(j=0;j<4-ct;j++)
				x[total++]=0;
			for(j=0;j<ct;j++)
				x[total++]=strtol(strndup(ipaddr+i+j,1), NULL, 16);
			i+=ct;
				
		}
	//	x[30]=x[28];
	//	x[31]=x[29];
	//	x[28]=0;
	//	x[29]=0;
		
	//	dst_ip->s6_addr[i]=atoi(strndup(ipaddr+(i*5),4)); 
	}
	printf("\n\n");
	for(i=0;i<8*4;i++)
		printf("|%d",x[i]);
		
	
	return 0;
}
}*/