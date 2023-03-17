import React from 'react';
import { useEffect, useState } from 'react';
import { PointElement, LineElement, LinearScale, CategoryScale, Chart } from "chart.js";
import { Line } from 'react-chartjs-2';

Chart.register(CategoryScale);
Chart.register(LinearScale);
Chart.register(PointElement);
Chart.register(LineElement);

function PlayerStats(props) {

    const [playerStats, setplayerStats] = useState({
        id:'',
        name: '',
        allLegAvg: 0,
        bestLegAvg: 0,
        highScore: 0,
        bestDartCount170: 0,
        bestDartCount301: 0,
        bestDartCount501: 0,
        highestCheckout: 0,
        matchCount: 0,
        legCount: 0
    });

    useEffect(() => {
        async function fetchData() {
            let mounted = true;
            let url = 'https://localhost:7141/players/' + props.playerId;
            await fetch(url)
                .then(response => response.json())
                .then(response => {
                    if (mounted) {
                        setplayerStats(response)
                    }
                })
                .catch(error => console.log('error: ' + error))
            return () => mounted = false;
        }
        fetchData();
    }, [])

    return (
        <div className='mychart'>
            <table>
                <tr>
                    <td>{playerStats.name}</td>
                    <td></td>
                </tr>
                <tr>
                    <td>Gespeicherte Spiele</td>
                    <td>{playerStats.matchCount}</td>
                </tr>
                <tr>
                    <td>Gespeicherte Legs</td>
                    <td>{playerStats.legCount}</td>
                </tr>
                <tr>
                    <td>Duchschnitt &uuml;ber alle Legs</td>
                    <td>{playerStats.allLegAvg}</td>
                </tr>
                <tr>
                    <td>Bestes Leg</td>
                    <td>{playerStats.bestLegAvg}</td>
                </tr>
                <tr>
                    <td>H&ouml;chster Wurf</td>
                    <td>{playerStats.highScore}</td>
                </tr>
                <tr>
                    <td>H&ouml;chster Checkout</td>
                    <td>{playerStats.highestCheckout}</td>
                </tr>
                <tr>
                    <td>Bestes Spiel 170</td>
                    <td>{playerStats.bestDartCount170}</td>
                </tr>
                <tr>
                    <td>Bestes Spiel 301</td>
                    <td>{playerStats.bestDartCount301}</td>
                </tr>
                <tr>
                    <td>Bestes Spiel 501</td>
                    <td>{playerStats.bestDartCount501}</td>
                </tr>
            </table>
        </div>
    )
}

export default PlayerStats;
