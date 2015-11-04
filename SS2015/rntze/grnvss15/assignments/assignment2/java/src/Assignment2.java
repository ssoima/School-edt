import java.net.InetAddress;
import java.net.UnknownHostException;

public class Assignment2 {

/*====================================TODO===================================*/
	/**
	 * This function parses the ip address from a string into a network byte
	 * order representation
	 *
	 * @param hwaddr The ip address as string
	 *
	 * @return A newly allocated byte array filled with the ip address
	 */
	private static byte[] parseIP(String hwaddr) {
		return null;
	}

	/**
	 * This function initializes an ethernet header
	 *
	 * @param dstmac The destination mac
	 * @param srcmac The mac of this host
	 * @param ethertype The ethernet type for the header
	 *
	 * @return A newly allocated byte array filled with the header
	 */
	private static byte[] buildEtherHeader(byte[] dstmac, byte[] srcmac,
								int ethertype) {
		return null;
	}

	/**
	 * This function initializes an ipv6 header
	 * This encompasses the version and everything give as parameter
	 *
	 * @param dstIp the destination ip
	 * @param srcIp the ip of this host
	 * @param next The value for the next header field
	 * @param len The length of the ip payload
	 * @param hlim The hoplimit for the packet
	 * @param label The flow label for the ip packet
	 *		The last 20 bit of the label should be used!
	 *		The label should be in proper byte order already
	 *
	 * @return A newly allocated byte array filled with the ip header
	 */
	private static byte[] buildIP6Header(byte[] dstIp, byte[] srcIp,
				byte next, short len, byte hlim, int label) {
		return null;
	}

	/**
	 * This function initializes the icmpv6 header and the neighbor
	 * discovery payload
	 *
	 * @param dstIp the destination ip
	 * @param srcMac the mac of this host
	 *
	 * @return A newly allocated byte array filled with the payload
	 */
	private static byte[] buildIcmp6Ndisc(byte[] dstIp, byte[] srcMac) {
		return null;
	}

	/**
	 * This function checks whether the ethernet header may be for a
	 * neighbor advertisment which is sent as response to our neighbor
	 * discovery
	 *
	 * @param buffer A buffer containing the received frame
	 * @param offset The offset of the frame in the buffer
	 * @param mymac A byte array containing the mac of this host
	 *
	 * @result true on success
	 *	   false on fail
	 */
	private static boolean myEtherHeader(byte[] buffer, int offset, byte[]
									mymac) {
		return false;
	}

	/**
	 * This function check whether the ip ethernet payload is an ipv6 header
	 * and if it may be a neighbor advertisment sent in response to our
	 * neighbor discovery
	 *
	 * @param buffer A byte array containing the received packet
	 * @param offset The offset of the packet in the buffer
	 * @param myIp A byte array containing the ip of this host
	 * @param dstIp A byte array containing the destination ip
	 *
	 * @return true on success
	 *	   false on fail
	 */
	private static boolean myIP6Header(byte[] buffer, int offset,
						byte[] myIp, byte[] dstIp) {
		return false;
	}

	/**
	 * This function checks whether the ip payload is a neighbor
	 * advertisment sent to us
	 *
	 * @param buffer A byte array containing the received packet
	 * @param offset The offset of the ip payload
	 * @param dstIp The destination ip
	 *
	 * @return true on success
	 *	   false on fail
	 */
	private static boolean isMyNeighborAdvert(byte[] buffer, int offset,
							byte[] dstIp) {
		return false;
	}

	/**
	 * This function checks if the frame received is a neighbor advertisment
	 * set as response to our neighbor discovery. If it is, it returns a
	 * byte array filled with the target mac
	 * It calls the more specialized functions in succession to check their
	 * parts and advances over optional ipv6 extension headers.
	 *
	 * @param buffer A byte array filled with the received frame
	 * @param length The size of the received frame (<= buffer.length)
	 * @param srcmac The mac of this host
	 * @param srcip The ip of this host
	 * @param dstip The destination ip
	 *
	 * @return null on fail
	 *	   A newly allocated byte array filled with the mac address of
	 *	   the destination node
	 */
	private static byte[] retrieveMac(byte[] buffer, int length,
				byte[] srcmac, byte[] srcip, byte[] dstip) {
		return null;
	}

/*===========================================================================*/

	public static void run(GRNVS_RAW sock, String dst, int timeout) {
		Timeout time = new Timeout(timeout*1000);
		byte[] recbuffer = new byte[1514];
		byte[] buffer = new byte[1514];
		byte[] dstip = null;
		byte[] srcip = null;
		byte[] dstmac = null;
		byte[] srcmac = null;
		byte[] ether_header = null;
		byte[] ip_header = null;
		byte[] payload = null;
		int ret;
		int length = 0;
		int checksum;

		if((dstip = parseIP(dst)) == null) {
			System.err.println("Your destination input format is broken, it should be: xx:xx:xx:xx:xx:xx");
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

/*===========================================================================*/
		sock.hexdump(buffer, length);

		while((ret = sock.read(recbuffer, time)) != 0) {
			if((dstmac = retrieveMac(recbuffer, ret, srcmac, srcip,
								dstip)) != null)
				break;
		}
		if(ret == 0) {
			//This is supposed to go to stdout, the tester needs
			//this. Do NOT change this
			System.out.println("Message timed out");
			return;
		}
/*====================================TODO===================================*/
	/*
	 *TODO: Print the retrieved mac address.
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


	public static void main(String[] argv) {
		Arguments args = new Arguments(argv);
		GRNVS_RAW sock = null;
		try{
			sock = new GRNVS_RAW(args.iface, 3);
			run(sock, args.dst, args.timeout);
			sock.close();
		}
		catch(Exception e) {
			System.out.println(e.getMessage());
			System.exit(1);
		}
	}
}
