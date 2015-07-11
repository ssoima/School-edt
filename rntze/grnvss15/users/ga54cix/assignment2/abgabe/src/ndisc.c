#include <argp.h>
#include <stdio.h>
#include <net/ethernet.h>
#include <string.h>
#include <stdlib.h>

#include "ndisc.h"


static char args_doc[] = "ADDRESS";

static char doc[] =
	"ndisc	the second programming excersize for GRNVS-SS15\n"
	"-i	the interface on which the magic packet should be sent (default: eth0)\n"
	"-t	the timeout that should be used for timed actions (default: 5)\n"
	"DST	the destination ip6 address\n";

enum fix_args {
	FIX_ARG_DST = 0,
	FIX_ARG_CNT
};

static struct argp_option options[] = {
	{
		"timeout",
		't',
		"timeout",
		0,
		0,
		0
	},
	{
		"interface",
		'i',
		"interface",
		0,
		0,
		0
	},
	{ 0 }
};

static error_t parse_opt(int key, char * arg, struct argp_state * state);

static struct argp argp = {
	options,
	parse_opt,
	args_doc,
	doc,
	0,
	0,
	0
};

static error_t parse_opt(int key, char * arg, struct argp_state * state)
{
	struct arguments * args = state->input;
	char * ptr;

	switch (key) {
	case ARGP_KEY_ARG:
		switch(state->arg_num) {
		case FIX_ARG_DST:
			args->dst = arg;
			break;
		default:
			return ARGP_ERR_UNKNOWN;
		}
		break;
	case 'i':
		args->interface = arg;
		break;
	case 't':
		args->timeout = (int)strtol(arg, &ptr, 10);
		if(*ptr)
			return EINVAL;
		break;
	default:
		return ARGP_ERR_UNKNOWN;
	}
	return 0;
}

int parse_args(struct arguments * args, int argc, char ** argv)
{
	memset(args, 0, sizeof(*args));
	if(argp_parse(&argp, argc, argv, 0, 0, args))
		return -1;
	if(!args->dst)
		return -1;
	if(!args->timeout)
		args->timeout=5;
	if(!args->interface)
		args->interface = "eth0";

	return 0;
}
