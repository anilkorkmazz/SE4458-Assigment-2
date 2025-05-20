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


## 🧪 Sample Test Messages



### ✈️ Query Flight

**User Input:**

> Is there a flight for 20 people from Istanbul to New York on September 15, 2025 between 08:00 - 12:00?

**Bot Output:**

✈️ Available Flights:  
1. Flight Code: FL-0010  
   ⏱️ Duration: 3 hours 30 minutes  
2. Flight Code: FL-0013  
   ⏱️ Duration: 4 hours 50 minutes

---

### 🎫 Buy Ticket

**User Input:**

> Buy a ticket for flight FL-0003 on September 15, 2025 for Tutku

**Bot Output:**

🎫 Ticket(s) created:  
1. Ticket Code: TK-D1E133

---

### 👥 Check Passenger List

**User Input:**

> Could you please check the passenger list for FL-0001 dated September 15, 2025?

**Bot Output:**

- Ali — Seat No: 1  
- Veli — Seat No: 2  
**Total Passengers:** 2

---

### ✅ Check-In

**User Input:**

> Check in for the flight FL-0003 on September 15, 2025 for the passenger named Nuray

**Bot Output:**

✅ Checked in successfully

---

## ⚠️ Challenges Encountered

---

### 🚧 Azure App Service Issue and Migration to Vercel

**Issue:**  
Initially, I tried to deploy the frontend application using Azure App Service. However, the page wouldn't load at all and kept throwing errors. Since there were no clear error messages, it was difficult to diagnose the problem.

**Solution:**  
Due to these issues, I migrated the frontend to Vercel. Vercel provided a much smoother deployment process and faster setup for the React-based project. The overall deployment became more stable and efficient.

---

### 🔧 500 Internal Server Error in Gateway

**Issue:**  
During integration, we encountered an unexpected 500 error in the Gateway.

**Root Cause:**  
Deserialization issues in OpenAI responses and missing fields in `AiResponseDto`.

**Resolution:**  
Improved logging and added better exception handling, which helped identify and resolve the issue.

---

## 📹 Video Demo

🎥 **Watch the video here:**  
[Demo Video](https://youtu.be/rUZjFJA-8CQ)



##  Deployment Links

| Layer         | URL                                                                 |
|---------------|----------------------------------------------------------------------|
| 🖥️ Frontend     | [https://se-4458-assignment-2.vercel.app](https://se-4458-assignment-2.vercel.app)         |
| 🌐 API Gateway | [https://airline-gateway.azurewebsites.net](https://airline-gateway.azurewebsites.net)     |
| 📡 Airline API | [https://anil-airline-api.azurewebsites.net/swagger](https://anil-airline-api.azurewebsites.net/swagger) |


