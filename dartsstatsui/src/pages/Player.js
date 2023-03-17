import React from 'react';
import { useParams } from 'react-router-dom';
import { Stack, Divider} from '@mui/material'
import CheckoutChart from '../components/CheckoutChart'
import WonVsLostChart from '../components/WonVsLostChart'
import LegAvgChart from '../components/LegAvgChart'
import TotalDartsChart from '../components/TotalDartsChart'
import '../App.css';
import PlayerStats from '../components/PlayerStats';


function Player() {
    let { playerId } = useParams();

    return (
        <div className='App-header'>
            <Stack sx={{ padding: '10px' }} direction='column' spacing={1} divider={<Divider orientation='vertical' />}>
                <PlayerStats playerId={playerId} />
                <CheckoutChart playerId={playerId} />
                <LegAvgChart playerId={playerId} />
                <TotalDartsChart playerId={playerId} />
                <WonVsLostChart playerId={playerId} />
            </Stack>
        </div>
    )
}

export default Player;

