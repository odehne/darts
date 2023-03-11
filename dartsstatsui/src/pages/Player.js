import React, { Component } from 'react';
import CheckoutChart from '../components/CheckoutChart'
import WonVsLostChart from '../components/WonVsLostChart'
import LegAvgChart from '../components/LegAvgChart'
import TotalDartsChart from '../components/TotalDartsChart'
import '../App.css';


class Player extends Component {
    constructor(props) {
        super(props);
        console.log(props.playerId);
    }

    render() {

        return (
            <div className='player'>
                <div className='App-header'>
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
            </div>
        )
    }
}

export default Player;