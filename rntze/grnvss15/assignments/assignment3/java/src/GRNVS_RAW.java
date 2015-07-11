public class GRNVS_RAW {
	static {
		System.loadLibrary("GRNVS");
	}

	private final int handle;

	private final native int getSocket(String dev, int level);
	private final native int write_(int handle, byte[] buffer, int size);
	private final native int read_(int handle, byte[] buffer, Timeout time);
	private final native int close_(int handle);
	private final native byte[] mac_(int handle);
	private final native byte[] ip_(int handle);
	private final native byte[] ip6_(int handle);

	private static final native void hexdump_(byte[] buffer, int size);
	private static final native int checksum_(byte[] hdr, int hdrOffset,
			byte[] payload, int payloadOffset, int payloadLength);

	public static final int SOCK_RAW = 3;
	public static final int SOCK_DGRAM = 2;

	/**
	 * Open a socket with libraw support
	 *
	 * @param dev The name of the device to use
	 * @param level The layer to use (SOCK_DGRAM for l3, SOCK_RAW for l2)
	 */
	public GRNVS_RAW(String dev, int level) {
		handle = getSocket(dev, level);
	}

	/**
	 * Write size bytes of data from buffer to the network
	 *
	 * @param buffer The buffer that contains the data
	 * @param size The number of bytes to write
	 *
	 * @return The number ob bytes written
	 */
	public final int write(byte[] buffer, int size) {
		return write_(handle, buffer, size);
	}

	/**
	 * Read data from the network into a buffer
	 *
	 * @param buffer A buffer to read the data into
	 * @param time A timeout object which specifies the timeout
	 *
	 * @return The number of bytes read
	 */
	public final int read(byte[] buffer, Timeout time) {
		return read_(handle, buffer, time);
	}

	public final byte[] getIP() {
		return ip_(handle);
	}

	public final byte[] getIPv6() {
		return ip6_(handle);
	}

	public final byte[] getMac() {
		return mac_(handle);
	}

	public final int close() {
		return close_(handle);
	}

	/**
	 * Write a hexdump of a byte-array to stderr
	 *
	 * @param buffer The array to dump
	 * @param size The number of bytes to dump
	 */
	public static final void hexdump(byte[] buffer, int size) {
		GRNVS_RAW.hexdump_(buffer, size);
	}

	/**
	 * Calculate the internet checksum for an ICMPv6 packet
	 * This requires the Checksum to be set to 0 befor calling this
	 *
	 * @param hdr A byte-array containing the IPv6 header for the packet
	 * @param hdrOffset The index of the first byte of the header in hdr
	 * @param payload A byte-array containing the IP-Payload for the packet
	 * @param payloadOffset The index of the first byte to use in payload
	 * @param payloadLength Then length of the payload for the checksum
	 *
	 * @return The ICMPv6 checksum for the given header and payload
	 */
	public static final int checksum(byte[] hdr, int hdrOffset,
			byte[] payload, int payloadOffset, int payloadLength) {
		return GRNVS_RAW.checksum_(hdr, hdrOffset, payload,
						payloadOffset, payloadLength);
	}

	protected void finalize() {
		close_(handle);
	}
}
