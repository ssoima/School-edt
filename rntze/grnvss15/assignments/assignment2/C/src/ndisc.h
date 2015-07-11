#ifndef _NDISC6_H_
#define _NDISC6_H_

struct arguments {
	char * interface;
	char * dst;
	int timeout;
};

int parse_args(struct arguments * args, int argc, char ** argv);

#endif /*_NDISC6_H_*/
