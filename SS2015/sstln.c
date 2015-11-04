/*
ss -tln 

ss: to another utility investigate sockets

-t, --tcp     Display TCP sockets.

State      Recv-Q Send-Q      Local Address:Port          Peer Address:Port   
ESTAB      0      0               127.0.0.1:45596            127.0.0.1:epmd    
CLOSE-WAIT 1      0           10.179.74.246:50945         209.20.75.76:http    
CLOSE-WAIT 1      0           10.179.74.246:57262         91.189.89.31:http    
ESTAB      0      0        ::ffff:127.0.0.1:epmd      ::ffff:127.0.0.1:45596  

-l, --listening
              Display only listening sockets (these are omitted by default).

Netid  State      Recv-Q Send-Q   Local Address:Port       Peer Address:Port   
nl     UNCONN     0      0                 rtnl:winbindd/7991                *       
nl     UNCONN     0      0                   16:wpa_supplicant/1151                *       
nl     UNCONN     0      0                   18:kernel                 *       
........ETC tot asa.......
p_raw  UNCONN     0      0                    *:wlan0                  *       
p_dgr  UNCONN     0      0              [34958]:wlan0                  *       
p_dgr  UNCONN     0      0                  [0]:*                      *       
u_str  LISTEN     0      128    /tmp/.s.PGSQL.5432 9492                  * 0      
u_str  LISTEN     0      5         /var/run/sdp 11528                 * 0      
........ETC tot asa.......
u_dgr  UNCONN     0      0                    * 1863                  * 11541  
u_dgr  UNCONN     0      0                    * 17495                 * 11541  
........ETC tot asa.......
tcp    UNCONN     0      0                    *:ipproto-48590               *:*            
tcp    UNCONN     0      0        10.179.74.246:manet                 *:*             
........ETC tot asa.......
tcp    LISTEN     0      128                  *:34643                 *:*            
tcp    LISTEN     0      128                 :::epmd                 :::*   
........ETC tot asa.......    

              
-n, --numeric
              Do not try to resolve service names.

Netid  State      Recv-Q Send-Q     Local Address:Port       Peer Address:Port 
u_str  ESTAB      0      0      @/tmp/dbus-5aCB7VP6N2 15111                 * 17590
u_str  ESTAB      0      0      @/tmp/dbus-5aCB7VP6N2 40501                 * 26304
........ETC tot asa.......
tcp    ESTAB      0      0              127.0.0.1:44399         127.0.0.1:44399 
tcp    ESTAB      0      0              127.0.0.1:45596         127.0.0.1:4369  
tcp    CLOSE-WAIT 1      0          10.179.74.246:50945      209.20.75.76:80    
tcp    CLOSE-WAIT 1      0          10.179.74.246:57262      91.189.89.31:80    
tcp    ESTAB      1208   0       2001:4ca0:0:f274:f1f2:5715:e586:f677:57662  2a00:1450:4001:805::1000:443   
tcp    ESTAB      0      0       ::ffff:127.0.0.1:4369   ::ffff:127.0.0.1:45596 

ss -nlt 

State      Recv-Q Send-Q        Local Address:Port          Peer Address:Port 
LISTEN     0      128                       *:34643                    *:*     
LISTEN     0      5                 127.0.1.1:53                       *:*     
LISTEN     0      128               127.0.0.1:631                      *:*     
LISTEN     0      128                       *:5432                     *:*     
LISTEN     0      5                 127.0.0.1:29754                    *:*     
LISTEN     0      50                        *:445                      *:*     
LISTEN     0      80                127.0.0.1:3306                     *:*     
LISTEN     0      50                        *:139                      *:*     
LISTEN     0      128                     ::1:631                     :::*     
LISTEN     0      128                      :::5432                    :::*     
LISTEN     0      50                       :::445                     :::*     
LISTEN     0      128                      :::5672                    :::*     
LISTEN     0      50                       :::139                     :::*     
LISTEN     0      128                      :::80                      :::*     
LISTEN     0      128                      :::4369                    :::*   

cat /proc/net/tcp

  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode                                                     
   0: 00000000:8753 00000000:0000 0A 00000000:00000000 00:00000000 00000000   116        0 14850 1 0000000000000000 100 0 0 10 0                     
   1: 0101007F:0035 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 1586 1 0000000000000000 100 0 0 10 0                      
   2: 0100007F:0277 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 51037 1 0000000000000000 100 0 0 10 0                     
   3: 00000000:1538 00000000:0000 0A 00000000:00000000 00:00000000 00000000  1001        0 15457 1 0000000000000000 100 0 0 10 0                     
   4: 0100007F:743A 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 1864 1 0000000000000000 100 0 0 10 0                      
   5: 00000000:01BD 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 48822 1 0000000000000000 100 0 0 10 0                     
   6: 0100007F:0CEA 00000000:0000 0A 00000000:00000000 00:00000000 00000000   999        0 9550 1 0000000000000000 100 0 0 10 0                      
   7: 00000000:008B 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 48823 1 0000000000000000 100 0 0 10 0                     
   8: 0100007F:B21C 0100007F:1111 01 00000000:00000000 00:00000000 00000000   116        0 14852 1 0000000000000000 20 4 30 10 -1                    
   9: F64AB30A:C701 4C4B14D1:0050 08 00000000:00000001 00:00000000 00000000  1000        0 85501 1 0000000000000000 32 4 33 10 -1                    
  10: F64AB30A:DFAE 1F59BD5B:0050 08 00000000:00000001 00:00000000 00000000  1000        0 37261 1 0000000000000000 22 4 30 10 -1   
*/
//sudo apt-get install build-essential manpages manpages-dev manpages-posix manpages-posix-dev
//sudo apt-get install build-essential manpages manpages-dev manpages-posix manpages-posix-dev
//http://www.techytalk.info/linux-system-programming-open-file-read-file-and-write-file/

/*
 ============================================================================
 Name        : sp_linux_copy.c
 Author      : Marko Martinović
 Description : Copy input file into output file
 ============================================================================
 */
 
#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>
#include <errno.h>
#include <sys/types.h>
#include <unistd.h>
 
#define BUF_SIZE 8192
 
  
int main(int argc, char ** argv)
{

printf("hallo\n");
int input_fd;
ssize_t ret_in;    /* Number of bytes returned by read() and write() */
char buffer[BUF_SIZE];      /* Character buffer */

/* Create input file descriptor */
input_fd = open ("/proc/net/tcp", O_RDONLY);
if (input_fd == -1) {
        perror ("open");
        return 2;
}

while((ret_in = read (input_fd, &buffer, BUF_SIZE)) > 0){
        printf("%s\n",buffer);}

close (input_fd);

  return EXIT_SUCCESS;
}