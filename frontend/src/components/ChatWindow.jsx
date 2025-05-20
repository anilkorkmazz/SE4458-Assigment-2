import { useEffect, useRef, useState } from "react";
import MessageBubble from "./MessageBubble";
import { sendMessage, listenMessages } from "../services/chatService";

export default function ChatWindow() {
  const [messages, setMessages] = useState([]);
  const [text, setText] = useState("");
  const bottomRef = useRef(null);

  useEffect(() => {
    const unsubscribe = listenMessages((msgs) => setMessages(msgs));
    return () => unsubscribe();
  }, []);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const handleSend = async () => {
    if (!text.trim()) return;
  
    const userMessage = {
      text,
      sender: "user",
      timestamp: Date.now(),
    };
  
    
    setText("");
  
    
    await sendMessage(userMessage);
  
    try {
      const response = await fetch("https://airline-gateway.azurewebsites.net/api/agent/message", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ message: text }),
      });
      
      const data = await response.json();
  
      // Format duration: minutes → hours + minutes
      const formatDuration = (minutes) => {
        const h = Math.floor(minutes / 60);
        const m = minutes % 60;
        return `${h} hour${h !== 1 ? "s" : ""}${m > 0 ? ` ${m} minute${m !== 1 ? "s" : ""}` : ""}`;
      };
  
      // ISO date → 15.09.2025
      const formatDate = (isoString) => {
        const d = new Date(isoString);
        return d.toLocaleDateString("tr-TR");
      };
  
      let botText = "";
  
      if (data.flights?.length > 0) {
        botText =
          "✈️ Available Flights:\n\n" +
          data.flights
            .map(
              (f, i) =>
                `${i + 1}. Flight Code: ${f.flightNumber}\n` +
                //`  ✈️ Departure: ${f.departureAirport} → Arrival: ${f.arrivalAirport}\n` +
                `  ⏱️ Duration: ${formatDuration(f.duration)}\n` 
                //`  📆 Date: ${formatDate(f.dateFrom)}`
            )
            .join("\n\n");
      
  
      } else if (data.flights?.length === 0) {
        botText = "⚠️ No flights found matching the given criteria.";
      }

      else if (data.ticketNumbers?.length > 0) {
        botText =
          "🎫 Ticket(s) created:\n\n" +
          data.ticketNumbers
            .map((t, i) => `${i + 1}. Ticket Code: ${t}`)
            .join("\n");
  
      } else if (data.passengers?.length > 0) {
        botText =
          data.passengers
            .map((p) => `${p.passengerName} — Seat No: ${p.seatNumber}`)
            .join("\n") +
          `\n\nTotal Passengers: ${data.totalCount || data.passengers.length}`;
  
      } else if (data.intent === "CheckIn" && data.status === "SUCCESS") {
        botText =
        "Check-in successful!\n\n" +
        `Passenger: ${data.passengerName}\n` +
        `Seat Number: ${data.seatNumber}`;

      } else if (data.message || data.status) {    
        botText = data.message || data.status;

      } else {
        botText = "No valid result found.";
      }
  
      await sendMessage({
        text: botText,
        sender: "bot",
        timestamp: Date.now(),
      });
  
    } catch (err) {
      console.error("🛑 API error:", err);
      await sendMessage({
        text: "⚠️ The system could not respond. Please try again.",
        sender: "bot",
        timestamp: Date.now(),
      });
    }
  };  
  

  return (
    <div className="h-screen flex flex-col bg-gray-100">
      {/* HEADER */}
      <header className="p-4 shadow bg-gradient-to-r from-blue-600 to-indigo-600 text-white">
        <h2 className="text-2xl font-semibold text-center tracking-wide">
          ✈️ AI Ticket Assistant
        </h2>
      </header>

      

      {/* MESSAGES */}
      <main className="flex-1 overflow-y-auto px-4 py-6">
        <div className="max-w-3xl mx-auto space-y-3">
          {messages.map((msg, index) => (
            <MessageBubble key={index} message={msg} />
          ))}
          <div ref={bottomRef} />
        </div>
      </main>


      {/* INPUT AREA */}
      <footer className="bg-white border-t py-4 shadow-inner">
        <div className="max-w-3xl mx-auto px-4 flex gap-2">
          <input
            type="text"
            value={text}
            onChange={(e) => setText(e.target.value)}
            placeholder="Type your message..."
            className="flex-1 px-4 py-2 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          <button
            onClick={handleSend}
            className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-lg shadow"
          >
            Send
          </button>
        </div>
      </footer>
    </div>
  );
}
