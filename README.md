# ✈️ Airline Ticketing AI Agent Chat App

This project implements an **AI-powered chat assistant** for airline ticketing operations. It fulfills the requirements of **SE4458 - Assignment 2** by integrating OpenAI with existing airline APIs behind a .NET-based API Gateway using Ocelot.

## 🔍 Features

- Natural language interface (via OpenAI GPT-3.5)
- Handles 4 intents:
  - `QueryFlight`
  - `BuyTicket`
  - `CheckIn`
  - `QueryPassengerList`
- Chat UI updates in real time via Firestore
- All API calls go through a centralized **.NET Gateway** using Ocelot

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
