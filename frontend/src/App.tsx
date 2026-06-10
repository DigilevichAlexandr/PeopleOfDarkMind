import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import Layout from './components/Layout';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import GamePage from './pages/GamePage';
import NewGamePage from './pages/NewGamePage';
import MapPage from './pages/MapPage';
import CharacterPage from './pages/CharacterPage';
import InventoryPage from './pages/InventoryPage';
import InvestigationPage from './pages/InvestigationPage';
import JournalPage from './pages/JournalPage';
import RelationsPage from './pages/RelationsPage';
import SettingsPage from './pages/SettingsPage';

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route element={<PrivateRoute><Layout /></PrivateRoute>}>
            <Route path="/game" element={<GamePage />} />
            <Route path="/new-game" element={<NewGamePage />} />
            <Route path="/map" element={<MapPage />} />
            <Route path="/character" element={<CharacterPage />} />
            <Route path="/inventory" element={<InventoryPage />} />
            <Route path="/investigation" element={<InvestigationPage />} />
            <Route path="/journal" element={<JournalPage />} />
            <Route path="/relations" element={<RelationsPage />} />
            <Route path="/settings" element={<SettingsPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
