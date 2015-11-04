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
	private static final int SOCK_RAW = 3;
	private static final int SOCK_DGRAM = 2;

	public GRNVS_RAW(String dev, int level) {
		handle = getSocket(dev, level);
	}

	public final int write(byte[] buffer, int size) {
		return write_(handle, buffer, size);
	}

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

	public static final void hexdump(byte[] buffer, int size) {
		GRNVS_RAW.hexdump_(buffer, size);
	}

	public static final int checksum(byte[] hdr, int hdrOffset,
			byte[] payload, int payloadOffset, int payloadLength) {
		return GRNVS_RAW.checksum_(hdr, hdrOffset, payload,
						payloadOffset, payloadLength);
	}

	protected void finalize() {
		close_(handle);
	}
}
