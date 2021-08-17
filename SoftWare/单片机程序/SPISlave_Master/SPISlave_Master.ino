/*
    SPI Master Demo Sketch
    Connect the SPI Master device to the following pins on the esp8266:

    GPIO    NodeMCU   Name  |   Uno
   ===================================
     15       D8       SS   |   D10
     13       D7      MOSI  |   D11
     12       D6      MISO  |   D12
     14       D5      SCK   |   D13

    Note: If the ESP is booting at a moment when the SPI Master has the Select line HIGH (deselected)
    the ESP8266 WILL FAIL to boot!
    See SPISlave_SafeMaster example for possible workaround

*/
#include <SPI.h>

class ESPMaster {
  private:
    uint8_t _ss_pin;

  public:
    ESPMaster(uint8_t pin): _ss_pin(pin) {}
    void begin() {
      pinMode(_ss_pin, OUTPUT);
      digitalWrite(_ss_pin, HIGH);
    }

    void writeData(uint8_t * data, size_t len) {
      uint8_t i = 0;
      digitalWrite(_ss_pin, LOW);
      while (len-- && i < 32) {
        SPI.transfer(data[i++]);
      }
      digitalWrite(_ss_pin, HIGH);
    }
};

ESPMaster esp(SS);

#define ELEMCNT 16 //定义数组元素个数
uint8_t a[ELEMCNT]={0}; //被操作的元素
uint8_t bb[ELEMCNT]={0};
uint8_t exceedA=0,exceedB=0;//记忆溢出位用的变量，其中exceedA为上一元素溢出值，exceedB为本元素溢出值
void LeftElem(int k)
{
  int i;  
  uint8_t Msk=0; //用于计算溢出位的掩码，如k=6时，其掩码值为00000011(二进制)  
  for (i=0;i<8-k;i++)
  {    
    Msk=Msk|(1<<i);  
  }   
  //移动所有数组元素,i值对应数组元素下标i-1
  for (i=0;i<ELEMCNT;i++)
  {    
    exceedB=(a[i]&(~Msk))>>(8-k);//计算溢出数据  
    exceedB = a[i]>>7;
    a[i]=a[i]<<1;    
    a[i]+=exceedA;
    exceedA=exceedB;  
  }
}


void setup() {
  Serial.begin(115200);
  SPI.begin();
  esp.begin();
  a[0] = 0x01;
  delay(1000);
}

void loop() {
  LeftElem(1);
  for(uint8_t k=0;k<ELEMCNT;k++)
  {
    bb[k] = a[ELEMCNT-k-1];
    }
  esp.writeData(bb,ELEMCNT);
  delay(10);
}
