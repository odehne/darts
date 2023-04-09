import React from 'react';
import { useParams } from 'react-router-dom';
import { Stack, Divider} from '@mui/material'
import CheckoutChart from '../components/CheckoutChart'
import WonVsLostChart from '../components/WonVsLostChart'
import LegAvgChart from '../components/LegAvgChart'
import TotalDartsChart from '../components/TotalDartsChart'
import '../App.css';
import PlayerStats from '../components/PlayerStats';
import BestLeg from '../components/BestLeg';
import LastWeeksTrend from '../components/LastWeeksTrend';


function Player() {
    let { playerId } = useParams();

    return (

        <div className='App-header'>
            <PlayerStats playerId={playerId} />
            <div className='serv'>
                <ul>
                    <li><BestLeg playerId={playerId} startValue='170' /></li>
                    <li><BestLeg playerId={playerId} startValue='301' /></li>
                    <li><BestLeg playerId={playerId} startValue='501' /></li>
                    <li><LastWeeksTrend playerId={playerId} startValue='170' /></li>
                    <li><LastWeeksTrend playerId={playerId} startValue='301' /></li>
                    <li><LastWeeksTrend playerId={playerId} startValue='501' /></li>
                    <li><CheckoutChart playerId={playerId} /></li>
                    <li><LegAvgChart playerId={playerId} /></li>
                    <li><TotalDartsChart playerId={playerId} /></li>
                    <li><WonVsLostChart playerId={playerId} /></li>
                </ul>
            </div>
        </div>
    )
}

export default Player;

