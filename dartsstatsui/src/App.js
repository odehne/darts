import './App.css';
import Player from './pages/Player'
import Navbar from './components/Navbar'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './pages/Home';



function App() {
  return (
      <div className="App">
          <Router>
              <Navbar />
              <Routes>
                  <Route path='/' element={<Home />}></Route>
                  <Route path='Player/:playerId' element={<Player />}></Route>
              </Routes>
          </Router>
    </div>
  );
}

export default App;
