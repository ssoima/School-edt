/****** IMPORT LIBRARIES *******/

// RS485 Communication
#include <SoftwareSerial.h>

// LEDs (Adafruit I2C)
#include <Wire.h>
#include "Adafruit_LEDBackpack.h"
#include "Adafruit_GFX.h"
#include <EEPROM.h>
#include <ArduinoJson.h>

/****** CONSTANTS AND PIN NUMBERS ******/
#define DebugEnabled     1   // Set to 1 in order to output debug information to Serial
#define SSerialRX        3   //Serial Receive pin
#define SSerialTX        4   //Serial Transmit pin
#define SSerialTxControl 2   //RS485 Direction control
#define PinButton        5   //Input pin for Button
#define SOTByte          60  // '<' Start of transmission character
#define EOTByte          62  // '>' End of transmission character
#define LedI2CAddress    0x72

#define Pin13LED         13  //Internal LED
#define epSlaveId        0 // EEPROM SlaveId Address
#define AnimationDelay   70

/****** OBJECTS ******/
SoftwareSerial RS485Serial(SSerialRX, SSerialTX); // RX, TX
Adafruit_BicolorMatrix ledMatrix = Adafruit_BicolorMatrix();

/****** GLOBAL VARIABLES ******/
byte SlaveId;
char rxBuffer[256];
int rxBufferLength;
bool rxToBuffer;

bool configMode = 0;
bool lastButtonState = 0;
bool buttonPressed = 0;

//Animation related
bool showArrow = 0;
byte animColor = 3;
int animIndex = 0;
unsigned long animPrevTime = 0;

PROGMEM static const uint8_t 
  ledBitmaps[][11] = 
  {
    { //Cross
      B10000001,
      B01000010,
      B00100100,
      B00011000,
      B00011000,
      B00100100,
      B01000010,
      B10000001,
    },
    { //Smiley
      B00111100,
      B01000010,
      B10100101,
      B10000001,
      B10100101,
      B10011001,
      B01000010,
      B00111100,
    },
    { //Neutral Smiley
      B00111100,
      B01000010,
      B10100101,
      B10000001,
      B10111101,
      B10000001,
      B01000010,
      B00111100,
    },
    { //Frown Smiley
      B00111100,
      B01000010,
      B10100101,
      B10000001,
      B10011001,
      B10100101,
      B01000010,
      B00111100,
    }     
  };    
  
PROGMEM static const uint8_t 
  ledArrowBitmaps[][11] = 
  {
    { 
      B00011000,
      B00011000,
      B00000000,
      B00000000,
      B00000000,
      B00000000,
      B00000000,
      B00011000,
    },
    { 
      B00011000,
      B00000000,
      B00000000,
      B00000000,
      B00000000,
      B00000000,
      B00011000,
      B00111100,
    },
    { 
      B00000000,
      B00000000,
      B00000000,
      B00000000,
      B00000000,
      B00011000,
      B00111100,
      B01111110,
    },
    { 
      B00000000,
      B00000000,
      B00000000,
      B00000000,
      B00011000,
      B00111100,
      B01111110,
      B00011000,
    },
    { 
      B00000000,
      B00000000,
      B00000000,
      B00011000,
      B00111100,
      B01111110,
      B00011000,
      B00011000,
    },  
    { 
      B00000000,
      B00000000,
      B00011000,
      B00111100,
      B01111110,
      B00011000,
      B00011000,
      B00011000,
    },
    { 
      B00000000,
      B00011000,
      B00111100,
      B01111110,
      B00011000,
      B00011000,
      B00011000,
      B00000000,
    },   
    { 
      B00011000,
      B00111100,
      B01111110,
      B00011000,
      B00011000,
      B00011000,
      B00000000,
      B00000000,
    },   
    { 
      B00111100,
      B01111110,
      B00011000,
      B00011000,
      B00011000,
      B00000000,
      B00000000,
      B00000000,
    }, 
    { 
      B01111110,
      B00011000,
      B00011000,
      B00011000,
      B00000000,
      B00000000,
      B00000000,
      B00000000,
    }, 
    { 
      B00011000,
      B00011000,
      B00011000,
      B00000000,
      B00000000,
      B00000000,
      B00000000,
      B00000000,
    },  
  };



/****** SETUP ******/
void setup()   
{  
  // Start the built-in serial port (for Serial Monitor)
  Serial.begin(9600);

  //Init IO pins
  pinMode(Pin13LED, OUTPUT); // Internal LED
  pinMode(SSerialTxControl, OUTPUT); // Transceiver send/receive
  
  digitalWrite(SSerialTxControl, LOW);  // Init Transceiver (Receive)
  ledMatrix.begin(LedI2CAddress);  // Init LED Matrix

  // Init Button Pin
  pinMode(PinButton, INPUT);
  digitalWrite(PinButton, HIGH);
  
  // Start the software serial port, to other slaves
  RS485Serial.begin(57600);   // set the baud rate 
  rxBufferLength = 0;
  rxToBuffer = 0;

  SlaveId = EEPROM.read(epSlaveId);
  if (SlaveId < 1 || SlaveId > 32)
  {
    SlaveId = 1;
    EEPROM.write(epSlaveId, SlaveId);
  }
  Serial.println(SlaveId);

  // Config Mode
  if (digitalRead(PinButton) == 0)
  {
    configMode = 1;
    ledMatrix.setTextSize(1);
    ledMatrix.setTextWrap(false);  // we dont want text to wrap so it scrolls nicely
    ledMatrix.setTextColor(LED_GREEN);
    ledMatrix.clear();
    ledMatrix.setCursor(0,0);
    ledMatrix.print(SlaveId);
    ledMatrix.writeDisplay();    
  }
  else
  {
    // Smile on startup :)
    ledMatrix.drawBitmap(0, 0, ledBitmaps[1], 8, 8, 3);
    ledMatrix.writeDisplay();
    delay(1000);
    ledMatrix.clear();
    ledMatrix.writeDisplay();    
  }
  

}


/****** LOOP ******/
void loop()
{
  if (configMode == 0)
  {
    // If data received via RS485, copy data to receiveBuffer
    if (RS485Serial.available()) 
    {
      digitalWrite(Pin13LED, HIGH);  // Show activity    
      
      int rxByte = RS485Serial.read();   // Read the byte
  
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
        Debug("SOT");
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
        Debug("EOT");
        rxToBuffer = 0;
        
        // Create JSON object
        StaticJsonBuffer<256> jsonBuffer;
        JsonObject& root = jsonBuffer.parseObject(rxBuffer);
        
        if (!root.success())
        {
          Debug("JSON Error");
          return;
        }
        
        int slaveId = root["id"];
        int cmd = root["cmd"];
        int color = root["color"];
        const char* value = root["value"];
  
        if (slaveId == SlaveId)
        {
          showArrow = 0;
          
          if (cmd == 0) // Clear Matrix
          {
            ledMatrix.clear();
            ledMatrix.writeDisplay();
          }
          else if (cmd == 1) // Fill Matrix
          {
            ledMatrix.clear();
            ledMatrix.fillRect(0,0, 8,8, color);
            ledMatrix.writeDisplay(); 
          }   
          else if (cmd == 2) // Animate Arrow
          { 
            animColor = color;
            showArrow = 1;
          }
          else if (cmd == 3) // Cross
          {
            ledMatrix.clear();
            ledMatrix.drawBitmap(0, 0, ledBitmaps[0], 8, 8, color);
            ledMatrix.writeDisplay();
          }
          else if (cmd == 4) // Text
          {
            ledMatrix.setTextSize(1);
            ledMatrix.setTextWrap(false);  // we dont want text to wrap so it scrolls nicely
            ledMatrix.setTextColor(color);
  
            int valueSize = strlen(value);
            valueSize = (valueSize * 8) * -1;
            
            for (int8_t x=0; x>=valueSize; x--) {
              ledMatrix.clear();
              ledMatrix.setCursor(x,0);
              ledMatrix.print(value);
              ledMatrix.writeDisplay();
              delay(60);
            }
          }          
          else if (cmd == 5) // Smiley
          {
            if (char(value[0]) == '1') // Happy
            {
              ledMatrix.clear();
              ledMatrix.drawBitmap(0, 0, ledBitmaps[1], 8, 8, color);
              ledMatrix.writeDisplay();
            }
            else if (char(value[0]) == '2') // Neutral
            {
              ledMatrix.clear();
              ledMatrix.drawBitmap(0, 0, ledBitmaps[2], 8, 8, color);
              ledMatrix.writeDisplay();
            }
            else if (char(value[0]) == '3') // Sad
            {
              ledMatrix.clear();
              ledMatrix.drawBitmap(0, 0, ledBitmaps[3], 8, 8, color);
              ledMatrix.writeDisplay();              
            }
          }   

        }
      }
  
    }
    else
    {
      digitalWrite(Pin13LED, LOW);
    } 
  }

  bool buttonState = digitalRead(PinButton);
  if (lastButtonState != buttonState)
  { 
    lastButtonState = buttonState;
    if (buttonState == 0) // Pressed
    {
      buttonPressed = 1;
    }
  }

  if (buttonPressed == 1 && rxToBuffer == 0)
  {
    Serial.println("Button pressed");
    buttonPressed = 0;

    if (configMode == 0)
    {
      String txString = String("{\"id\":0,\"sid\":");
      txString += SlaveId;
      txString += ",\"cmd\":11}";
      char txChars[40];
      txString.toCharArray(txChars, 40);
      Serial.write(txChars);
      RS485Send(txChars);      
    }
    else
    {
      SlaveId ++;
      if (SlaveId < 1 || SlaveId > 32)
      {
        SlaveId = 1;
      }      
      EEPROM.write(epSlaveId, SlaveId);
      Serial.print ("SlaveChange");
      Serial.print (SlaveId);

      ledMatrix.setTextSize(1);
      ledMatrix.setTextWrap(false);  // we dont want text to wrap so it scrolls nicely
      ledMatrix.setTextColor(LED_GREEN);
      ledMatrix.clear();
      ledMatrix.setCursor(0,0);
      ledMatrix.print(SlaveId);
      ledMatrix.writeDisplay();   
    }
  }

  if (millis() - animPrevTime >= AnimationDelay)
  {
    if (showArrow)
    {
      ledMatrix.clear();
      ledMatrix.drawBitmap(0, 0, ledArrowBitmaps[animIndex], 8, 8, animColor);
      ledMatrix.writeDisplay();
  
      animIndex++;
      if (animIndex >= 11)
      {
        animIndex = 0;
      }
     
    }
    animPrevTime = millis();     
  }

}

void RS485Send(char* bytes)
{
  Debug("Send");
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








