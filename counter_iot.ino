/*Ajustes no sensor 2*/

#include <LiquidCrystal_I2C.h>
#include <WiFi.h>
#include <HTTPClient.h>
#define BUTTON_PIN_BITMASK 0x2 // saida 2
LiquidCrystal_I2C lcd (0x27, 16,2);

int BUZZZER_PIN = 18;
int LED_PIN = 19;
int ir1 = 13;
int ir2 = 15;
int objectDetected1 = LOW;
int objectDetected2 = LOW;
int starttime;
int endtime;
int counter = 0; // sempre verificar se está setado para 0.
int passagerLimit = 10;
int timeOut = 120 * 1000; // dois minutos
int busCode = 1;
int driverCode = 1;
int incrementBtn = 25;
int decrementBtn = 33;
int confirmBtn = 26;
int btnDelay = 120;
RTC_DATA_ATTR int bootCount = 0;


const char WIFI_SSID[] = "***"; // Colocar seu o SSID do seu roteador
const char WIFI_PASSWORD[] = "***"; // Colocar a senha do Wi-Fi
String HOST_NAME = "https://busprime.azurewebsites.net"; // Colocar o Host name
String PATH_NAME = "/Contador"; // Colocar o nome do caminho



void setup() {
  esp_sleep_enable_ext0_wakeup(GPIO_NUM_2,1); //1 = High, 0 = Low
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Hello!");
  pinMode(ir1, INPUT);
  pinMode(ir2, INPUT);
  digitalWrite(ir1, HIGH);
  digitalWrite(ir2, HIGH);
  pinMode(incrementBtn, INPUT_PULLUP);
  pinMode(decrementBtn, INPUT_PULLUP);
  pinMode(confirmBtn, INPUT_PULLUP);
  Serial.begin(115200);
  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);
  lcd. begin ();
  lcd. backlight ();
  choosePassLimit();
  delay(150);
  chooseBusCode();
  delay(150);
  chooseDriverCode();
  starttime = millis();
  endtime = starttime;
  lcd.clear();
  Serial.println("Iniciando contagem");
  lcd.setCursor(0, 0);
  lcd.print("Iniciando");
  lcd.setCursor(0, 1);
  lcd.print("contagem ...");
  delay(1500);
  lcd.clear();
}

void choosePassLimit(){
  int iButtonState;
  int dButtonState;
  int cButtonState;
  // print out the button's state
  // LOW 0 HIGH1
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Selec. numero");
  while (true){
    iButtonState = digitalRead(incrementBtn);
    dButtonState = digitalRead(decrementBtn);
    cButtonState = digitalRead(confirmBtn);
    if (iButtonState == LOW){
      passagerLimit += 1;
      }
     if (dButtonState == LOW){
      passagerLimit -= 1;
      if (passagerLimit <= 0)
        passagerLimit = 1;
      }
     if (cButtonState == LOW){
      break;
      }
      
      Serial.print("Num. Passg.");
      Serial.println(passagerLimit);
      lcd.setCursor(0, 1);
      if (passagerLimit < 10){
        lcd.print("passageiros: 0");
        lcd.print(passagerLimit);

      }
      else{
        lcd.print("passageiros: ");
        lcd.print(passagerLimit);
      }
      delay(btnDelay);
    }
  }

void chooseBusCode(){
  int iButtonState;
  int dButtonState;
  int cButtonState;
  int busCodeLimit = 5;
  // print out the button's state
  // LOW 0 HIGH 1
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Selec. onibus");
  while (true){
    iButtonState = digitalRead(incrementBtn);
    dButtonState = digitalRead(decrementBtn);
    cButtonState = digitalRead(confirmBtn);
    if (iButtonState == LOW){
      busCode += 1;
      }
     if (dButtonState == LOW){
      busCode -= 1;
      if (busCode <= 0)
        busCode = 1;
      }
      if (busCode > busCodeLimit)
        busCode = busCodeLimit;
     if (cButtonState == LOW){
      break;
      }
      delay(btnDelay);
      Serial.print("Bus Code:");
      Serial.println(busCode);
      lcd.setCursor(0, 1);
      lcd.print("onibus: ");
      lcd.print(busCode);
    }
  }

void chooseDriverCode(){
  int iButtonState;
  int dButtonState;
  int cButtonState;
  int driverCodeLimit = 10;
  // print out the button's state
  // LOW 0 HIGH 1
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Selec. motorista");
  while (true){
    iButtonState = digitalRead(incrementBtn);
    dButtonState = digitalRead(decrementBtn);
    cButtonState = digitalRead(confirmBtn);
    if (iButtonState == LOW){
      driverCode += 1;
      }
     if (dButtonState == LOW){
      driverCode -= 1;
      if (driverCode <= 0)
        driverCode = 1;
      }
      if (driverCode > driverCodeLimit)
        driverCode = driverCodeLimit;
     if (cButtonState == LOW){
      break;
      }
      delay(btnDelay);
      Serial.print("Driver Code:");
      Serial.println(driverCode);
      lcd.setCursor(0, 1);
      if (driverCode < 10){
        lcd.print("Motorista: 0");
        lcd.print(driverCode);

      }
      else{
        lcd.print("Motorista: ");
        lcd.print(driverCode);
      }
      // lcd.print("Motorista: ");
      // lcd.print(driverCode);
    }
  }

void alert(){
    delay(500);
    tone(BUZZZER_PIN, 523, 200);
    digitalWrite(LED_PIN, HIGH);
    delay(500);
    digitalWrite(LED_PIN,LOW);
    noTone(BUZZZER_PIN);
}

void sendLcdBlockMsg(String msg1 = "", String msg2 = ""){
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print(msg1);
      lcd.setCursor(0, 1);
      lcd.print(msg2);
  }


void sendToDB(int contP, int idBus, int idDriver) {
  const int connTimeOut = 10;
  int connTimer = 0;
  sendLcdBlockMsg("Connecting Wifi", "...");
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  Serial.println("Connecting");
  while(WiFi.status() != WL_CONNECTED){
    delay(500);
    Serial.printf(".");
    if (connTimer > connTimeOut){
      Serial.println("Conn. Timeout!");
      sendLcdBlockMsg("Conn. Timeout!", "Wifi failed...");
      delay(2000);
      return;
      }
  }
  Serial.printf("Connected to WiFi network with IP Address; ");
  Serial.println(WiFi.localIP());
  String queryString = "";
  String inputString1 = "?contP=";
  String inputString2 = "&idBus=";
  String inputString3 = "&idMot=";
  queryString = inputString1 + String(contP) + inputString2 + String(idBus) + inputString3 + String(idDriver);
  HTTPClient http;
  Serial.printf("Requisition: %s%s%s\n", HOST_NAME.c_str(), PATH_NAME.c_str(), queryString.c_str());
  http.begin(HOST_NAME + PATH_NAME + queryString); // HTTP  
  http.addHeader("Content-Type", "text/plain"); 
  int httpCode = http.POST(HOST_NAME + PATH_NAME + queryString);

  // httpCode will be negative on error
  if(httpCode > 0){
    // file found at server
    if(httpCode == HTTP_CODE_OK){
      String payload = http.getString();
      Serial.println(payload);
      sendLcdBlockMsg("Success sending", "data to DB!");
      delay(1000);
    }else{
      // HTTP header has been send and Server response header has been handled
      Serial.printf("[HTTP] GET ... code: %d\n", httpCode);
      Serial.println("Get code");
      Serial.println(httpCode);
    }
  }else{
    Serial.printf("[HTTP] Code... failed, error: %s\n", http.errorToString(httpCode).c_str());
    sendLcdBlockMsg("Fail to send data", "to DB...");
    delay(1500);
    sendLcdBlockMsg("[HTTP] Code:", http.errorToString(httpCode).c_str());
  }
  http.end();
  delay(3000);
}
void loop() {
  lcd.setCursor (0, 0);
  lcd.print ( "Onibus " );
  lcd.print(busCode);
  int objectDetected1;
  int objectDetected2;
  int startRetTime;
  int endRetTime;
  const int toleranceTime1 = 1400; // antes 1400
  const int toleranceTime2 = 1150; // antes 1150
  const int entranceDelay1 = 800; // antes 1000
  const int entranceDelay2 = 450; // antes 400
  const int nextObjDelay = 800;// antes 700
  
  while ((endtime - starttime) <= timeOut){
    objectDetected1 = digitalRead(ir1);
    objectDetected2 = digitalRead(ir2);
    startRetTime = millis();
    endRetTime = startRetTime;
    delay(100); // respiro antes de começar a processar
    if(objectDetected1 == LOW){
       Serial.println("LOW A1");
       counter += 1;
       objectDetected1 = HIGH;
       Serial.printf("StateA is %d\n", objectDetected1);
       // LOW is 0
       // HIGH is 1
       delay(entranceDelay1);
       while ((endRetTime - startRetTime) <= toleranceTime1){
        objectDetected1 = digitalRead(ir1);
        if(objectDetected1 == LOW){
          Serial.println("False entrace 1");
          counter -= 1;
          break;
        }
        endRetTime = millis();
       }
      }
    else if(objectDetected2 == LOW){
      Serial.println("LOW A2");
      counter -= 1;
      objectDetected2 = HIGH;
      Serial.printf("StateA is %d\n", objectDetected1);
      delay(entranceDelay2);
      while ((endRetTime - startRetTime) <= toleranceTime2){
        objectDetected2 = digitalRead(ir2);
        if(objectDetected2 == LOW){
          Serial.println("False entrace 2");
          counter += 1;
          break;
        }
        endRetTime = millis();
       }
    }
    else{
      endtime = millis();
      return;
    }
    if (counter < 0) counter = 0;
    if (counter >= passagerLimit){
      sendLcdBlockMsg("Lim. reached!");
      break;
      }
    delay(nextObjDelay);
    endtime = millis();
    lcd.setCursor (0, 1);
    if (counter < 10){
        lcd.print("Num. pessoa: 0");
        lcd.print(counter);
      }
     else{
        lcd.print("Num. pessoa: ");
        lcd.print(counter);
      }
    Serial.printf("Control count: %d\n", counter);
  }
  if  ((endtime - starttime) > timeOut){
    sendLcdBlockMsg("Time Out!");
   }
  lcd.setCursor(0, 1);
  lcd.print("Total:");
  lcd.print(counter);
  Serial.printf("Final count: %d\n", counter);
  for (int i = 0; i <= 10; i++){
    alert();
    }
  Serial.printf("Final count: %d\n", counter);
  Serial.printf("Counter Runtime: %d\n", (endtime - starttime));
  // sendLcdBlockMsg("Sending to DB...");
  sendToDB(counter, busCode, driverCode); // Modificar para aceitar codigo do motorista
  sendLcdBlockMsg("Going to sleep", "Good night!");
  delay(5000);
  lcd.noBacklight();
  esp_deep_sleep_start(); 
}
