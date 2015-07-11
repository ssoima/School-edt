import java.net.InetAddress;
import java.net.UnknownHostException;

public class Assignment3 {

	static final int EXCEEDED = 1;
	static final int UNREACHABLE = 2;
	static final int ECHOREPLY = 3;

/*====================================TODO===================================*/
	/**
	 * This function parses the ip address from a string into a network byte
	 * order representation
	 *
	 * @param hwaddr The ip address as string
	 *
	 * @return A newly allocated byte array filled with the ip address
	 *	   Or null if the String is not an ipv6-address
	 */
	private static byte[] parseip(String hwaddr) {
		return null;
	}

	/**
	 * This function initializes an ipv6 header
	 * This encompasses the version and everthing given as a parameter
	 *
	 * @param dstIp the destination ip
	 * @param srcIp the ip of this host
	 * @param next The value for the next header field
	 * @param len The length of the ip payload
	 * @param hlim The hoplimit for the packet
	 * @param label The flow label for the ip packet
	 *		The last 20 bits of the label should be used!
	 *		The label should be in proper byte order already
	 *
	 * @return A newly allocated byte array filled with the ip header
	 */
	private static byte[] buildip6Header(byte[] dstIp, byte[] srcIp,
				byte next, short len, byte hlim, int label) {
		return null;
	}

	/**
	 * This function initializes the icmpv6 header and the echo request
	 *
	 * @param id The id for the echo request
	 * @param seq the sequence for the echo request
	 *
	 * @return A newly allocated byte array filled with the payload
	 */
	private static byte[] buildEchoRequest(short id, short seq) {
		return null;
	}


	/**
	 * This function checks whether packet starts with an ipv6 header
	 * and if it may be a packet sent in response to our echo request
	 *
	 * @param buffer A byte array containing the received packet
	 * @param offset The offset of the packet in the buffer
	 * @param myIp A byte array containing the ip of this host
	 *
	 * @return true on success
	 *	   false on fail
	 */
	private static boolean myip6Header(byte[] buffer, int offset,
					   byte[] myIp) {
		return true;
	}

	/**
	 * This function checks whether the ip payload is an echo reply sent in
	 * response to our echo request
	 *
	 * @param buffer The buffer containing the received packet
	 * @param offset The index of the first byte to check
	 * @param id The id used in the echo request
	 * @param seq The sequence number used in the echo request
	 * @param dstIp the destination ip for the echo request
	 *
	 * @return true on success
	 *	   false on fail
	 */
	private static boolean myEchoResponse(byte[] buffer, int offset,
					      short id, short seq,
					      byte[] dstIp) {
		return true;
	}

	/**
	 * This function checks if the received packet is an destination
	 * unreachable icmp message sent in response to our echo request
	 *
	 * @param buffer The buffer containing the received packet
	 * @param offset The index of the first byte to check
	 * @param len The length read from the ip-header of the packet
	 * @param sent A buffer containing the echo request sent
	 * @param slen The size of the sent packet
	 *
	 * @return true on success
	 *	   false on fail
	 */
	private static boolean myUnreachable(byte[] buffer, int offset,
					     int len, byte[] sent, int slen) {
		return true;
	}


	/**
	 * This function checks if the received packet is a time
	 * exceeded icmp message sent in response to our echo request
	 *
	 * @param buffer The buffer containing the received packet
	 * @param offset The index of the first byte to check
	 * @param len The length read from the ip-header of the packet
	 * @param sent A buffer containing the echo request sent
	 * @param slen The size of the sent packet
	 *
	 * @return true on success
	 *	   false on fail
	 */
	private static boolean myTimeExceeded(byte[] buffer, int offset,
					      int len, byte[] sent, int slen) {
		return true;
	}



	/**
	 * This function checks whether the received packet is for us
	 * It calls the more specialized functions in succession to check their
	 * parts and advances over optional ipv6 extension headers
	 *
	 * @param buffer A buffer containing the received packet
	 * @param len The size of the received packet
	 * @param id The id used for our echo request
	 * @param seq The sequence number used for our echo request
	 * @param myIp The ip address of this host used for the echo request
	 * @param dstIp Th destination of our echo request
	 * @param sent A buffer containing the sent packet
	 * @param slen The size of the sent packet
	 * @param hopIp A buffer in which the ip of the hop should be copied to
	 *
	 * @return 0 if the packet was not for us
	 *	  The fitting value from the packetTypes defines
	 */
	private static int getHopIp(byte[] buffer, int len, short id, short seq,
				    byte[] myIp, byte[] dstIp, byte[] sent,
				    int slen, byte[] hopIp)
	{
		return 0;
	}

	/**
	 * This function sends out an echo request packet to the network and
	 * reads from the network until either a response to the packet is found
	 * or the timeout expires
	 *
	 * It also converts the ip address read from the network into a human
	 * readable representation
	 *
	 * @param buffer A buffer containing the packet to be sent
	 * @param len The length of the packet to send
	 * @param myIp The ip address of this host used for the echo request
	 * @param dstIp The destination of the echo request
	 * @param id The id used for the echo request
	 * @param seq The sequence number used for the echo request
	 * @param ipName A Stringbuffer into which the human readable form of
	 * 	  the ip address should be written (empty:  ipName.setlength(0)
	 * @param sock The GRNVS_RAW socket object to use for network IO
	 * @param timeout The timeout to apply given in seconds
	 *
	 * @return 0 if the request timed out
	 *	  The fitting value from the packetTypes defines
	 */
	private static int runAttempt(byte[] buffer, int len, byte[] myIp,
				      byte[] dstIp, short id, short seq,
				      StringBuffer ipName,
				      GRNVS_RAW sock, int timeout) {
		byte[] recBuffer = new byte[1514];
		byte[] hopIp = new byte[16];
		Timeout time;
		return 0;
	}

/*===========================================================================*/

	public static void run(GRNVS_RAW sock, String dst, int timeout,
			       int attempts, int hopLimit) {
		byte[] buffer = new byte[1514];
		byte[] dstIp;
		byte[] srcIp;
		byte[] ipHeader;
		byte[] payload;
		StringBuffer ipName = new StringBuffer();

		if((dstIp = parseip(dst)) == null) {
			System.err.println("Your destination input format is broken, it should be: xx:xx:xx:xx:xx:xx");
			return;
		}


/*====================================TODO===================================*/

		/* TODO:
		 * 1) Initialize the addresses required to build the packet
		 * 2) Loop over hoplimit and attempts
		 * 3) Build and send the packet for each iteration
		 * 4) Print the hops found in the specified format
		 */
/*===========================================================================*/

	}


	public static void main(String[] argv) {
		Arguments args = new Arguments(argv);
		GRNVS_RAW sock = null;
		try{
			sock = new GRNVS_RAW(args.iface, 2);
			run(sock, args.dst, args.timeout, args.attempts,
			    args.hoplimit);
			sock.close();
		}
		catch(Exception e) {
			e.printStackTrace();
			System.out.println(e.getMessage());
			System.exit(1);
		}
	}
}
