import { collection, addDoc, query, orderBy, onSnapshot } from "firebase/firestore";
import db from "../firebase/firebaseConfig";
import { serverTimestamp } from "firebase/firestore";

const messagesRef = collection(db, "messages");

export async function sendMessage(message) {
  try {
    await addDoc(messagesRef, {
      ...message,
      timestamp: serverTimestamp(), 
    });
  } catch (error) {
    console.error("Mesaj gönderilemedi:", error);
  }
}

export function listenMessages(callback) {
  const q = query(messagesRef, orderBy("timestamp"));
  const unsubscribe = onSnapshot(q, (snapshot) => {
    const msgs = snapshot.docs.map((doc) => {
      const data = doc.data();
      console.log("📥 Gelen mesaj:", data); 
      return data;
    });
    callback(msgs);
  });
  return unsubscribe;
}

