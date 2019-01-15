#include <Ethernet.h>
#include <SPI.h>
#include <MFRC522.h>

#define RST_PIN 9
#define SDA_PIN 4 // slave for mfrc522

byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
IPAddress ip(192,168,137,218);
char server[] = "192.168.137.1";

EthernetClient client;
String readString;
char str[32] = "";
byte readCard[4];
MFRC522 mfrc522(SDA_PIN, RST_PIN);

void setup()
{
  Ethernet.begin(mac, ip);
  Serial.begin(9600);
  mfrc522.PCD_Init(); 
  
  delay(2000);


 
}

void loop()
{
  Readcard();
 
}

void Readcard(){
  while(mfrc522.PICC_IsNewCardPresent() && mfrc522.PICC_ReadCardSerial()){
    for (int i = 0; i < 4; i++) { 
     readCard[i] = mfrc522.uid.uidByte[i];
     //Serial.print(readCard[i], HEX);
    }
     array_to_string(readCard,4,str);
    Serial.println(str);
    /*
   if (mfrc522.uid.uidByte[0] != readCard[0] || 
    mfrc522.uid.uidByte[1] != readCard[1] || 
    mfrc522.uid.uidByte[2] != readCard[2] || 
    mfrc522.uid.uidByte[3] != readCard[3] ) {
    for (int i = 0; i < 4; i++) { 
     readCard[i] = mfrc522.uid.uidByte[i];
     //Serial.print(readCard[i], HEX);
    }
     array_to_string(readCard,4,str);
    Serial.println(str);

   int res = client.connect(server, 80);

  //Serial.println(res);

  delay(2000);
  
    
    
   
  }
  */
  mfrc522.PICC_HaltA();
  mfrc522.PCD_StopCrypto1();
  
}
 client.stop(); //stop client
}
void array_to_string(byte array[], unsigned int len, char buffer[])
{
    for (unsigned int i = 0; i < len; i++)
    {
        byte nib1 = (array[i] >> 4) & 0x0F;
        byte nib2 = (array[i] >> 0) & 0x0F;
        buffer[i*2+0] = nib1  < 0xA ? '0' + nib1  : 'A' + nib1  - 0xA;
        buffer[i*2+1] = nib2  < 0xA ? '0' + nib2  : 'A' + nib2  - 0xA;
    }
    buffer[len*2] = '\0';
} 
 
