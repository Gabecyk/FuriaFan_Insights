import './App.css';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import FormularioFuria from './FormularioFuria';
import PaginaResposta from './PaginaResposta';
import NavBar from './NavBar';

function App() {
  return (
    <Router>
      <div className="App">
        <NavBar />
        <Routes>
          <Route path="/" element={<FormularioFuria />} />
          <Route path="/result" element={<PaginaResposta />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;