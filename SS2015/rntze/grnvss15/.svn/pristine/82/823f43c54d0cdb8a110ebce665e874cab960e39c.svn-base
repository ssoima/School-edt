import java.io.*;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.UnknownHostException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Arrays;

public class Assignment4 {

    /**
     * This function returns the IP address from an address string or host name
     *
     * @param host The address string or host
     * @return A newly allocated byte array filled with the ip address
     * Or null if the String is not an ipv6-address
     */
    private static InetAddress getip(String host) {
        try {
            return InetAddress.getByName(host);
        } catch (UnknownHostException e) {
            return null;
        }
    }


/*====================================TODO===================================*/

    /**
     * Encode a string to an UTF-8 byte array using the standard library
     *
     * @param str String to Encode
     * @return UTF-8 encoded str, or null on error
     */
    static byte[] encodeUTF8(String str) {
        return null;
    }


    /**
     * Decode a byte buffer as UTF-8 using the standard library
     *
     * @param buf buffer to decode
     * @return decoded string or null on error
     */
    static String decodeUTF8(byte[] buf) {
        return null;
    }


    /**
     * Convert an input byte array to a netstring and write it to an output
     * stream.
     *
     * The input byte array must first be encoded as netstring:
     * <length of data in decimal>":"<data>","
     *
     * @param data The data to encode
     * @param out  Steam to write to
     */
    static void sendNetstring(OutputStream out, byte[] data) throws IOException {
    }

    /**
     * Read a netstring from an input stream and return its content as a
     * byte array.
     * The netstring may be fragmented, so this function might need to
     * call read() multiple times.
     * Since TCP is stream oriented, there may also be more than one
     * netstring buffered. This function must NOT read into the next
     * netstring, so it can be read later without problems.
     * This function must validate the netstring and strip the length
     * (including the ':') and the trailing ','.
     * It returns the contents as a byte array.
     *
     * @param reader The input stream to read from
     * @return The data of the netstring as newly allocated byte array or
     *         null on error
     */
    static byte[] recvNetstring(InputStream reader) {
        return null;
    }

    /**
     * This function creates a ServerSocket that listens on the given port,
     * ready to accept connections.
     *
     * Setting a timeout with ServerSocket.setSoTimeout of 5s is recommended.
     * Allowing address reuse with ServerSocket.setReuseAddress is recommended.
     *
     * @param port The port to listen on
     * @return The new Serversocket
     */
    static ServerSocket startListen(int port) {
        return null;
    }

    /**
     * This function should request for a message to be fetched by the server.
     *
     * This is part of phase one of the protocol.
     *
     * @param sock The control connection socket
     * @param nick The nickname to use
     * @param port The port to request for the data connection
     * @return The token sent by the server
     */
    static byte[] requestTransfer(Socket sock, String nick, int port) {
        return null;
    }

    /**
     * This function transfers the message in the second connection
     *
     * @param sock  The data connection socket
     * @param nick  The nick as sent in the previous connection
     * @param msg   The message to send to the server
     * @param token The token sent by the server in the first phase of the
     *              protocol
     * @return The data token
     */
    static byte[] sendMessage(Socket sock, String nick, byte[] msg,
                              byte[] token) {
        return null;
    }

    /**
     * This function commits the message after it has been transferred in the
     * data connection.
     *
     * @param sock The control connection socket
     * @param dtoken The data token returned by the server after the message
     *               has been transferred.
     * @param msglen The length of the msg sent to the server
     * @return True if the message was stored by the server
     */
    static boolean commitMessage(Socket sock, byte[] dtoken, int msglen) {
        return false;
    }
/*===========================================================================*/

    /**
     * This is the entry function for asciiclient.
     * It establishes the connection to the server and opens a listening
     * port.
     * It uses requestTransfer, sendMessage and commitMessage to post a
     * unicode message to the server.
     *
     * @param dst  The IP of the server in ASCII representation. IPv6 MUST be
     *             supported, support for IPv4 is optional.
     * @param port The server port to connect to
     * @param nick The nickname to use in the protocol
     * @param msg  The message to send
     */
    public static void run(String dst, int port, String nick, String msg) {

        InetAddress dstIp;

        if ((dstIp = getip(dst)) == null) {
            System.err.println("Could not get IP address for destination");
            return;
        }


/*====================================TODO===================================*/
/*===========================================================================*/
    }


    public static void main(String[] argv) {
        Arguments args = new Arguments(argv);
        String msg = args.msg;
        if (args.file != null) {
            try {
                msg = new String(Files.readAllBytes(Paths.get(args.file)));
            } catch (IOException e) {
                System.err.format("Could not open the file: %s\n", e.getMessage());
                return;
            }
        }

        try {
            run(args.dst, args.port, args.nick, msg);
        } catch (Exception e) {
            e.printStackTrace();
            System.out.println(e.getMessage());
            System.exit(1);
        }
    }
}
