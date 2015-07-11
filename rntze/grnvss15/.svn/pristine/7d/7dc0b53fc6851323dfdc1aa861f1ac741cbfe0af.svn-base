
public class Assignment1 {

/*====================================TODO===================================*/
	/**
	 * TODO: Allocate a buffer of the correct size and initialize the
	 * ethernet header here.
	 */
	private static byte[] buildEtherHeader(byte[] dstmac, byte[] srcmac) {
		return null;
	}

	/**
	 * TODO: Allocate a buffer of the correct size and initialize the WoL
	 * payload here.
	 */
	private static byte[] buildWolPayload(byte[] dstmac) {
		return null;
	}

	/**
	 * TODO: Parse the MAC address (string) and return the result as byte
	 * array for use in packet building. Return null on error.
	 */
	private static byte[] parseMAC(String hwaddr) {
		return null;
	}
/*===========================================================================*/

	public static void run(GRNVS_RAW sock, String dst) {
		byte[] buffer = new byte[1514];
		byte[] dstmac;
		byte[] srcmac;
		byte[] header;
		byte[] payload;
		int length;

		if((dstmac = parseMAC(dst)) == null) {
			System.err.println("Your destination input format is broken, it should be: xx:xx:xx:xx:xx:xx");
			return;
		}

/*====================================TODO===================================*/
		//TODO: buildwrite  ether-header
		// -> use buildEtherHeader()
		// -> getMac() in GRNVS_RAW returns your source address
		// -> determine the correct ethertype and take care of endianess

		//TODO: build WoL payload
		// -> use buildWolPayload()

		//TODO: Concatinate the ether header and the WoL payload into
		// buffer.
		// Determine the size of the WoL packet and set it here.
		length = 0;
		sock.hexdump(buffer, length);
		sock.write(buffer, length);
/*===========================================================================*/
	}



	public static void main(String[] argv) {
		Arguments args = new Arguments(argv);
		GRNVS_RAW sock = null;
		try{
			sock = new GRNVS_RAW(args.iface, 3);
			run(sock, args.dst);
			sock.close();
		}
		catch(Exception e) {
			System.out.println(e.getMessage());
			System.exit(1);
		}
	}
}
