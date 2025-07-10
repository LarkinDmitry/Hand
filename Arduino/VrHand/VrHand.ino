#include <SoftwareSerial.h>
SoftwareSerial BTSerial(4, 2); // RX, TX (SoftwareSerial для общения с HC-05)

const int ledPin = 13; // Встроенный светодиод на 13 пине
bool ledState = false;
int analogOutPins[] = {3, 11, 10, 9, 6, 5}; // Пины для аналогового выхода (должны поддерживать ШИМ) ладошка, большой, указательный, средний, безымянный, мизинец

void setup() {
  for (int i = 0; i < 6; i++) {    
    pinMode(analogOutPins[i], OUTPUT);
  }
  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, ledState); // Изначально светодиод выключен
  
  BTSerial.begin(115200);  // Скорость связи с HC-05
  while (!BTSerial);
  BTSerial.println("READY");
}

void loop() {
  if (BTSerial.available() > 0) {
    String command = BTSerial.readStringUntil('\n'); // Читаем всю строку
    command.trim(); // Удаляем лишние пробелы и символы
    
    if (command.equalsIgnoreCase("ON")) {
      ledState = true;
      digitalWrite(ledPin, HIGH);
    } 
    else if (command.equalsIgnoreCase("OFF")) {
      ledState = false;
      digitalWrite(ledPin, LOW);
      
      for (int i = 0; i < 6; i++) {    
        analogWrite(analogOutPins[i], 0);
      }
    }
    else if (command.equalsIgnoreCase("STATUS")) {
      BTSerial.println("READY");
    }
    else if (command.startsWith("DV") && command.length() == 8){
      for (int i = 0; i < 6; i++) {
        char c = command.charAt(2 + i);
        int value = c - 33; // смещение назад
        if (value < 0 || value > 100) {
          value = 0; // не корректный результат (ожидается 0...100)
        }
        int pwmValue = map(value, 0, 100, 0, 255);
        analogWrite(analogOutPins[i], pwmValue);
      }
    }
  }
}