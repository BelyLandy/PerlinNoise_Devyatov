# Система процедурной генерации ландшафта с помощью perlin noise - текстуры в Unity 6.

Работу выполнил Девятов Денис Сергеевич БПИ-238.

---

Данный проект предоставляет возможность генерации 3D ландшафта с полноценным графическим интерфейсом для его удобной настройки.

Это полноценный инструмент для создания вашей игры, готовый к расширению и интеграции с другими проектами.

## Как начать работу?

Зайдите в проект, выберите сцену Scenes/PerlinNoiseBase.

В окне Иерархии выберите PerlinGenerator

![image](https://github.com/user-attachments/assets/b2ec49c2-496d-46f0-9c5f-9cd756e2b225)

И в окне Инспектора перед нами открывается окно настроек нашего ландшафта.

![image](https://github.com/user-attachments/assets/35176f0d-5830-43fc-b2ea-d2083003a532)

![image](https://github.com/user-attachments/assets/bf32de8b-a928-4a1e-a481-152857c6b965)

![image](https://github.com/user-attachments/assets/cd554d57-af5e-4d5d-ae71-9b3ccd93e932)

![image](https://github.com/user-attachments/assets/e7d5a175-9db3-4239-ac10-00d9d5d900c0)

![image](https://github.com/user-attachments/assets/bd563834-0a23-4609-a67a-cdf8119a249d)

![image](https://github.com/user-attachments/assets/3858e25e-5a63-461c-bbdc-48fe2dda575f)

## Быстрый старт.

1) Для быстрого старта нажмите кнопку "Сгенерировать новый плейн" в окне Инспектора.

В игровой сцене вы увидите гладкий плейн с водой.

![image](https://github.com/user-attachments/assets/c35d5508-c58c-45d2-98fc-79d4afc8bd38)

2) Затем нажмите кнопку "Сгенерировать шум" в окне Инспектора.

Картинка в верхней правой части экрана показывает сгенерированную текстуру.

![image](https://github.com/user-attachments/assets/49978ff4-4a5c-4f6f-92cd-da3ceb4e468f)

3) Нажмите на кнопку "Применить шум к мешу" в окне Инспектора.

Мы сгенерировали ландшафт!

![image](https://github.com/user-attachments/assets/63c35adc-d363-4e8c-93f2-e933b47cc47c)

Для его удобного осмотра нажмите кнопку Play. Зажмите ПКМ и двигайте мышью, чтобы вращать камерой. Колёсиком мыши можно приближать или отдалять камеру.

![image](https://github.com/user-attachments/assets/f1066f1b-7b89-41c4-aca6-82ee0c89bd71)

4) В Инспекторе нажмем кнопку "Сгенерировать деревья".

![image](https://github.com/user-attachments/assets/1d0a1f14-1760-4b04-a75b-30ec7eb71521)

5) Сохраним наш сгенерированный ландшафт.

В Инспекторе найдем кнопку "Экспортировать текстуру шума".

Затем там же найдите окно "Сохранение префаба", нажмите галочки для сохранения воды и деревьев. И нажмите кнопку "Сохранить мэш как Prefab"

![image](https://github.com/user-attachments/assets/60f9260a-b5b6-4457-bdd2-42318dd52ea2)

В папке Assets/NoisePrefabs мы найдем свой ландшафт.

![image](https://github.com/user-attachments/assets/46898071-594d-478d-b0ad-6312cc8992b7)














