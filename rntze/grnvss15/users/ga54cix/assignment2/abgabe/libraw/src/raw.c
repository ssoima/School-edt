#include <arpa/inet.h>
#include <linux/if_arp.h>
#include <linux/if_packet.h>
#include <net/ethernet.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/ioctl.h>
#include <sys/socket.h>
#include <unistd.h>
#include <errno.h>
#include <sys/select.h>
#include <time.h>
#include <ifaddrs.h>

#include "raw.h"
#include "timespec.h"


static int ifindex = -1;
static uint8_t mac[ETH_ALEN];
static struct in_addr ip;
static struct in6_addr ip6;

static void grnvs_get_addresses(const char * ifname)
{
	struct ifaddrs * ifa;
	struct ifaddrs * ifc;

	if(getifaddrs(&ifa) < 0) {
		fprintf(stderr, "could not determine interface addresses: %s\n",
							strerror(errno));
		exit(-1);
	}

	for(ifc = ifa; ifc; ifc=ifc->ifa_next) {
		if(strcmp(ifc->ifa_name, ifname) || ifc->ifa_addr == NULL)
			continue;
		if(ifc->ifa_addr->sa_family == AF_INET) {
			memcpy(&ip,
				&((struct sockaddr_in*)ifc->ifa_addr)->sin_addr,
				sizeof(ip));
		}
		if(ifc->ifa_addr->sa_family == AF_INET6) {
			memcpy(&ip6,
				&((struct sockaddr_in6*)ifc->ifa_addr)->sin6_addr,
				sizeof(ip6));
		}
	}

	freeifaddrs(ifa);
}

int grnvs_open(const char * ifname, int layer)
{
	struct sockaddr_ll sa;
	struct ifreq if_idx;
	int fd;

	memset(&ip6, 0, sizeof(ip6));

	if(layer != SOCK_DGRAM && layer != SOCK_RAW) {
		fprintf(stderr, "Could not open socket: %s\n",
							strerror(EINVAL));
		exit(-1);
	}

	if (0 > (fd = socket(AF_PACKET, layer, htons(ETH_P_ALL)))) {
		fprintf(stderr, "socket() failed: %s\n", strerror(errno));
		exit(-1);
	}

	memset(&if_idx, 0, sizeof(if_idx));
	strcpy(if_idx.ifr_name, ifname);
	if (ioctl(fd, SIOCGIFINDEX, &if_idx) < 0) {
		fprintf(stderr, "ioctl() failed: %s\n", strerror(errno));
		exit(-1);
	}

	ifindex = if_idx.ifr_ifindex;

	memset(&if_idx, 0, sizeof(if_idx));
	strcpy(if_idx.ifr_name, ifname);
	if (ioctl(fd, SIOCGIFHWADDR, &if_idx) < 0) {
		fprintf(stderr, "ioctl() failed: %s\n", strerror(errno));
		exit(-1);
	}

	memcpy(&mac, if_idx.ifr_hwaddr.sa_data, ETH_ALEN);

	grnvs_get_addresses(ifname);

	sa.sll_family = AF_PACKET;
	sa.sll_ifindex = ifindex;
	if(layer == SOCK_RAW)
		sa.sll_protocol = htons(ETH_P_ALL);
	else
		sa.sll_protocol = htons(ETH_P_IP);

	if (bind(fd, (struct sockaddr *)&sa, sizeof(sa))) {
		fprintf(stderr, "bind() failed: %s\n", strerror(errno));
		exit(-1);
	}

	return fd;

}

ssize_t grnvs_read(int fd, void * buf, size_t maxlen, unsigned int * timeout)
{
	ssize_t len;
	int ret;
	fd_set rfd, rfds;
	struct timespec time;
	struct timespec before, after;
	struct timespec * tp = &time;

	if(ifindex < 0) {
		fprintf(stderr, "tried to read from closed socket, aborting\n");
		exit(-1);
	}

	if (!timeout) {
		tp = NULL;
	}
	else {
		if (*timeout <= 0)
			return 0;
		timespecmset(&time, *timeout);
	}

	FD_ZERO(&rfds);
	FD_SET(fd, &rfds);

	do {
		rfd = rfds;
		if (timeout)
			clock_gettime(CLOCK_MONOTONIC, &before);

		ret = pselect(fd+1, &rfd, NULL, NULL, tp, 0);

		if (timeout) {
			clock_gettime(CLOCK_MONOTONIC, &after);
			timespecsub(&after, &before);
			timespecsub(&time, &after);
			*timeout = time.tv_sec*1000 + time.tv_nsec/1000000;
		}

		if (ret == -1) {
			if (errno == EINTR)
				continue;
			fprintf(stderr, "pselect() failed: %s\n",
								strerror(errno));
			exit(-1);
		}

		if (ret == 0) {
			*timeout = 0;
			return 0;
		}

		len = read(fd, buf, maxlen);
	} while (0 > len && errno == EINTR);

	if(0 > len) {
		fprintf(stderr, "read() failed: %s\n", strerror(errno));
		exit(-1);
	}

	return len;
}

ssize_t grnvs_write(int fd, const void * buf, size_t len)
{
	ssize_t ret;
	if(ifindex < 0) {
		fprintf(stderr, "tried to write on closed socket, aborting\n");
		exit(-1);
	}


	do {
		ret = write(fd, buf, len);
	} while (0 > ret && errno == EINTR);

	if (0 > ret) {
		fprintf(stderr, "write() failed: %s\n", strerror(errno));
		exit(-1);
	}

	return ret;
}

int grnvs_close(int fd)
{
	ifindex = -1;
	memset(&mac, 0, sizeof(mac));
	memset(&ip6, 0, sizeof(ip6));
	ip.s_addr = 0;
	return close(fd);
}

const uint8_t  * grnvs_get_hwaddr(int fd)
{
	(void) fd;
	if(ifindex < 0)
		return NULL;
	return mac;
}


struct in_addr grnvs_get_ipaddr(int fd)
{
	(void) fd;
	return ip;
}

const struct in6_addr  * grnvs_get_ip6addr(int fd)
{
	(void) fd;
	if(ifindex < 0)
		return NULL;
	return &ip6;
}
