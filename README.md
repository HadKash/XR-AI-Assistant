# XR AI Assistant

An interactive **XR AI Assistant** built in **Unity** that combines **voice recognition, OpenAI integration, and passthrough camera vision** to create a conversational mixed reality experience.

NOTE: Many thanks to Valem for this tutorial

---

## Features

-  **Wake Word Detection** – Activate the assistant hands-free using Meta’s Voice SDK.  
-  **Voice Transcription UI** – See your live speech transcription and microphone input levels.  
-  **ChatGPT Integration** – Make natural language requests to the OpenAI API for intelligent responses.  
-  **Visual Scene Description** – Uses the **Passthrough Camera API** to let ChatGPT describe what the user sees in the real world.  
-  **Text-to-Speech (TTS)** – AI responses are spoken back using the **Meta Voice SDK** for a fully immersive XR experience.

---

## Technologies Used

- **Unity** (XR Development)
- **C#**
- **OpenAI API** (GPT-4o)
- **Meta XR SDK**
- **Meta Voice SDK (Wit.ai)**
- **Passthrough Camera API**
- **TextMeshPro**

---

## Platform

Designed and built for **Meta Quest 3 / Quest 2** (Android build).  
> To run this project, ensure that your Unity build platform is set to **Android** and that your headset is connected in **Developer Mode**.

---

## How It Works

1. Say the **wake word** to activate the assistant.  
2. Ask a question or say “describe what’s in front of me.”  
3. The assistant captures your view using the **Passthrough Camera**, sends it to **ChatGPT**, and returns a spoken AI description.  
4. Responses are displayed in a floating UI that follows the headset’s view.

---

## Demo

https://github.com/user-attachments/assets/fc2391eb-509d-4838-ac6e-26568d5bb40e




