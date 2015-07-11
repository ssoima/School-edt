#include <argp.h>
#include <stdio.h>
#include <net/ethernet.h>
#include <string.h>
#include <stdlib.h>

#include "wol.h"


static char args_doc[] = "DST";

static char doc[] =
	"wol	the first programming excersize for GRNVS-SS15\n"
	"-i	the interface on which the magic packet should be sent\n"
	"DST	the destination mac address\n";

enum fix_args {
	FIX_ARG_DST = 0,
	FIX_ARG_CNT
};

static struct argp_option options[] = {
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
	if(!args->interface)
		args->interface = "eth0";

	return 0;
}
