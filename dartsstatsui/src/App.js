import './App.css';
import Navbar from './components/Navbar'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './pages/Home';
import Player from './pages/Player';


function App() {
  return (
      <div className="App">
          <Router>
              <Navbar />
              <Routes>
                  <Route exact path='/' element={<Home />}></Route>
                  <Route exact path='/Player' element={<Player playerId="fb3fc2b2-a01b-4dd6-99e9-838262a8a614" />}></Route>
                  {/*<Route exact path='/Player' element={<Player playerId="mywerweId" />}></Route>*/}
                  {/*<Route exact path='/Player' element={<Player playerId="fb3fc2b2-a01b-4dd6-99e9-838262a8a614" />}></Route>*/}
                  {/*<Route exact path='/Player' element={<Player playerId="ed53c011-d9ff-4223-8053-1817713e16d5" />}></Route>*/}
              </Routes>
          </Router>
      
    </div>
  );
}

export default App;
