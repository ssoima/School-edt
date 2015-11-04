/****** IMPORT LIBRARIES *******/

// RS485 Communication
#include <SoftwareSerial.h>

// Ethernet shield
#include <SPI.h>
#include <Ethernet2.h>
#include <SD.h>
#include <ArduinoJson.h>

/****** CONSTANTS AND PIN NUMBERS ******/
#define DebugEnabled     1  // Set to 1 in order to output debug information to Serial
#define SSerialRX        3   //Serial Receive pin

// !!!!!!!! PIN CHANGE !!!!!!!

#define PinSD            4   //SPI SlaveSelect for SD card reader
#define SSerialTX        5   //Serial Transmit pin
#define SSerialTxControl 2   //RS485 Direction control
#define SOTByte          60  // '<' Start of transmission character
#define EOTByte          62  // '>' End of transmission character

#define PinButton        6   //Input pin for Button
#define Pin13LED         13  //Internal LED
#define ServerPort       23  //Ethernet Server Port
#define rxBufferSize     192

/****** OBJECTS ******/
SoftwareSerial RS485Serial(SSerialRX, SSerialTX); // RX, TX
EthernetServer server(ServerPort);

/****** GLOBAL VARIABLES ******/
char rxBuffer[rxBufferSize];
int rxBufferLength = 0;
bool rxToBuffer = 0;
bool txToSlaves = 0;

//DEMO
bool lastButtonState;
int demoIndex;


/****** SETUP ******/
void setup()
{
  // Start the built-in serial port (for Serial Monitor)
  Serial.begin(9600);

  Serial.println(F("Import configuration..."));
  // Open SD Card for reading config
  if (!SD.begin(PinSD)) {
    Serial.println(F("Error opening SD card!"));
    return;
  }

  // Open and read config file
  File configFile = SD.open("pblcfg.txt");
  if (configFile) 
  {
    int lineNo = 0;
    
    byte mac[6];
    IPAddress ip;
    IPAddress sn;
    IPAddress gw;

    while (configFile.available())
    {
      //Read one line of file to buffer
      byte rxByte = configFile.read();
      rxBuffer[rxBufferLength] = rxByte;
      rxBufferLength++;
      rxBuffer[rxBufferLength] = 0;

      if (char(rxByte) == '\n')
      {
        StaticJsonBuffer<70> jsonBuffer;
        JsonObject& root = jsonBuffer.parseObject(rxBuffer);
        if (!root.success())
        {
          Debug("JSON Error");
          return;
        }
        
        switch(lineNo)
        {
          case 0:
            mac[0] = strtol(root["mac"][0], NULL, 16);
            mac[1] = strtol(root["mac"][1], NULL, 16);
            mac[2] = strtol(root["mac"][2], NULL, 16);
            mac[3] = strtol(root["mac"][3], NULL, 16);
            mac[4] = strtol(root["mac"][4], NULL, 16);
            mac[5] = strtol(root["mac"][5], NULL, 16);          
          break;
          case 1:
            ip[0] = root["ip"][0];
            ip[1] = root["ip"][1];
            ip[2] = root["ip"][2];
            ip[3] = root["ip"][3];
          break;
          case 2:
            sn[0] = root["sn"][0];
            sn[1] = root["sn"][1];
            sn[2] = root["sn"][2];
            sn[3] = root["sn"][3];
          break;
          case 3:
            gw[0] = root["gw"][0];
            gw[1] = root["gw"][1];
            gw[2] = root["gw"][2];
            gw[3] = root["gw"][3];          
          break;
        }
        
        rxBufferLength = 0;
        lineNo ++;
      }
    } 
    configFile.close();

    Serial.println(F("Done"));  
    Serial.println(F("Initialize Network..."));

    Serial.print(mac[0], HEX);
    Serial.print(':');
    Serial.print(mac[1], HEX);
    Serial.print(':');
    Serial.print(mac[2], HEX);
    Serial.print(':');
    Serial.print(mac[3], HEX);
    Serial.print(':');
    Serial.print(mac[4], HEX);
    Serial.print(':');        
    Serial.println(mac[5], HEX);

    Serial.print(ip[0]);
    Serial.print('.');
    Serial.print(ip[1]);
    Serial.print('.');
    Serial.print(ip[2]);
    Serial.print('.');
    Serial.println(ip[3]);

    Serial.print(sn[0]);
    Serial.print('.');
    Serial.print(sn[1]);
    Serial.print('.');
    Serial.print(sn[2]);
    Serial.print('.');
    Serial.println(sn[3]);  

    Serial.print(gw[0]);
    Serial.print('.');
    Serial.print(gw[1]);
    Serial.print('.');
    Serial.print(gw[2]);
    Serial.print('.');
    Serial.println(gw[3]);  
  
    Ethernet.begin(mac, ip, gw, sn);
    server.begin(); // Start server
    Serial.println(F("Done"));  
  }
  else {
    Serial.println(F("Error reading file from SD card!"));
    return;
  }

  //Init IO pins
  pinMode(PinButton, INPUT);
  digitalWrite(PinButton, HIGH);
  pinMode(Pin13LED, OUTPUT); // Internal LED
  pinMode(SSerialTxControl, OUTPUT); // Transceiver send/receive
  
  digitalWrite(SSerialTxControl, LOW);  // Init Transceiver (Receive) 
  
  // Start the software serial port, to other slaves
  RS485Serial.begin(57600);   // set the data rate

  //DEMO
  lastButtonState = 1;
  demoIndex = 0;
}


/****** LOOP ******/
//#define msg1_1 "{\"id\":1,\"cmd\":0}"
//#define msg1_2 "{\"id\":2,\"cmd\":10,\"value\":\"Hello World!\"}"
//#define msg2_1 "{\"id\":1,\"cmd\":4,\"color\":3}"
//#define msg2_2 "{\"id\":2,\"cmd\":4}"


void loop()   
{
  bool buttonState = digitalRead(PinButton);
  if (lastButtonState != buttonState)
  { 
    lastButtonState = buttonState;
    if (buttonState == 0) // Pressed
    {
      Serial.println(F("Button"));
//      if (demoIndex == 0)
//      {
//        RS485Send(msg1_1);
//        RS485Send(msg1_2);        
//      } 
//      else if (demoIndex == 1)
//      {
//        RS485Send(msg2_1);
//        RS485Send(msg2_2);
//      }      

      demoIndex++;
      if (demoIndex == 2)
      {
        demoIndex = 0;
      }
    }
  }

  //Look for data received by Ethernet
  EthernetClient client = server.available();

  // Read if there is data available from a client
  if (client) {
    if (client.available()) 
    {
      int rxByte = client.read();   // Read the byte
      
      // Start of transmission char
      if (rxByte == SOTByte)
      {
        Debug("ETH SOT");
        digitalWrite(SSerialTxControl, HIGH);  // Enable RS485 Transmit
        txToSlaves = 1;
      }
      // Write data to RS485 once SOT has been received
      if (txToSlaves == 1) 
      {
        RS485Serial.write(rxByte);      
      }
      // End of transmission char
      if (rxByte == EOTByte)
      {
        txToSlaves = 0;
        Debug("ETH EOT");
        
        delay(10);  
        digitalWrite(SSerialTxControl, LOW);  // Disable RS485 Transmit 
        delay(10);
        
        // Write message including received string back to TCP client
        client.write("ETH Sent");
      }
    }
  }

  //Look for data from Slaves
  if (RS485Serial.available())  
   {
    int rxByte = RS485Serial.read();    // Read received byte

    if (DebugEnabled == 1)
    {
      Serial.write(rxByte);
      Serial.write(" (0x");
      Serial.print(rxByte, HEX);
      Serial.println(")");
    }
    
    // Start of transmission char
    if (rxByte == SOTByte)
    {
      Debug("RS SOT");
      rxToBuffer = 1;
      rxBufferLength = 0;
    }
    // Receive into buffer once SOT has been received
    if (rxToBuffer == 1 && rxByte != SOTByte && rxByte != EOTByte) 
    {
      rxBuffer[rxBufferLength] = char(rxByte);
      rxBufferLength++;
      rxBuffer[rxBufferLength] = 0;    
    }
    // End of transmission char
    if (rxByte == EOTByte)
    {
      Debug("RS EOT");
      rxToBuffer = 0;

      // Crap: Copy rxBuffer to new buffer as JSON parseObject destroys our
      // rxBuffer so that we cant forward the message if it is a button press.
      char rxBuffer2[rxBufferSize];
      strncpy(rxBuffer2, rxBuffer, rxBufferSize);

      // Create JSON object
      StaticJsonBuffer<200> jsonBuffer;
      JsonObject& root = jsonBuffer.parseObject(rxBuffer);
      if (!root.success())
      {
        Debug("JSON Error");
        return;
      }
      
      int slaveId = root["id"];
      int sourceId = root["sid"];
      int cmd = root["cmd"];

      if (slaveId == 0)
      {
        server.write(SOTByte);
        server.write(rxBuffer2);
        server.write(EOTByte);
      }
    }
  }
  
}

void RS485Send(char* bytes)
{
  digitalWrite(SSerialTxControl, HIGH);  // Enable RS485 Transmit
  RS485Serial.write(SOTByte);
  RS485Serial.write(bytes);
  RS485Serial.write(EOTByte);

  delay(10);  
  digitalWrite(SSerialTxControl, LOW);  // Disable RS485 Transmit 
  delay(10);
}

void Debug(char* message)
{
  if (DebugEnabled == 1)
  {
    Serial.println(message); 
  }
}

