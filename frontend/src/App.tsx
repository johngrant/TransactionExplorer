import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import { Header } from "./components/Header";
import { ThemeProvider } from "./components/ThemeProvider";
import { HomePage } from "./pages/HomePage";
import { TransactionDetailsPage } from "./pages/TransactionDetailsPage";

export default function App() {
  return (
    <ThemeProvider>
      <Router>
        <div className="min-h-screen bg-background">
          <Header />
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/transaction/:id" element={<TransactionDetailsPage />} />
          </Routes>
        </div>
      </Router>
    </ThemeProvider>
  );
}
