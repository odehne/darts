import logo from './logo.svg';
import './App.css';
import CheckoutChart from './components/CheckoutChart'
import WonVsLostChart from './components/WonVsLostChart'
import LegAvgChart from './components/LegAvgChart'
import TotalDartsChart from './components/TotalDartsChart'

function App() {
  return (
    <div className="App">
      <header className="App-header">
       {/*<img src={logo} className="App-logo" alt="logo" />*/}
              <div>
                 <h2>Checkout pro leg</h2>
                 <CheckoutChart />
              </div>
              <div>
                 <h2>Gewonnen vs Verloren</h2>
                 <WonVsLostChart />
              </div>
              <div>
                  <h2>Durchschnitt Score pro leg</h2>
                  <LegAvgChart />
              </div>
              <div>
                  <h2>Anzahl der Darts pro leg</h2>
                  <TotalDartsChart />
              </div>
      </header>
    </div>
  );
}

export default App;
