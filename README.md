# ✈️ Airline Ticketing AI Agent Chat App

This project implements an **AI-powered chatbot** for a flight ticketing system.  
The chatbot interprets **natural language messages** and uses **OpenAI’s GPT-3.5 model** to identify the user’s intent.  
All requests are routed through a **.NET Ocelot API Gateway**, which then interacts with the **Midterm APIs deployed on Azure**.

---

### ✅ Supported Intents

- ✈️ **QueryFlight** – Find available flights  
- 🎫 **BuyTicket** – Purchase tickets  
- 🪪 **CheckIn** – Assign seat number to passenger  
- 👥 **QueryPassengerList** – View passengers on a flight  


---

## 🏗️ Architecture

```text
User Message (React Chat UI)
        ↓
Message written to Firestore
        ↓
POST to API Gateway (/api/agent/message)
        ↓
OpenAiService.cs → OpenAI GPT-3.5 API
        ↓
Extracted Intent & Parameters (AiResponseDto)
        ↓
AirlineApiService.cs → Midterm API (via Ocelot)
        ↓
Midterm API Response
        ↓
Bot Message written to Firestore
        ↓
UI automatically updates in real time

```
---

## 📝 Assumptions

- User identity is static (no login required in frontend)
- JWT authentication is used in backend, token cached for 10 mins
- All APIs (except initial login) are accessed strictly via the Gateway as per assignment rules

---
