import React, { Component } from 'react';
import { Box, Stack, Divider, Grid } from '@mui/material'
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

            <Stack sx={{ border: '1px solid', padding: '10px' }} direction='column' spacing={1} divider={<Divider orientation='vertical' flexitem />}>
                <h3>Gewonnen vs Verloren</h3>
                <WonVsLostChart />
                <h3>Checkout pro leg</h3>
                <CheckoutChart />
                <h3>Durchschnitt Score pro leg</h3>
                <LegAvgChart />
                <h3>Anzahl der Darts pro leg</h3>
                <TotalDartsChart />
            </Stack>

            //<div className='player'>
            //    <div className='App-header'>
            //        <h3>Checkout pro leg</h3>
            //        <CheckoutChart />
            //    </div>
            //    <div>
            //        <h2>Gewonnen vs Verloren</h2>
            //        <WonVsLostChart />
            //    </div>
            //    <div>
            //        <h2>Durchschnitt Score pro leg</h2>
            //        <LegAvgChart />
            //    </div>
            //    <div>
            //        <h2>Anzahl der Darts pro leg</h2>
            //        <TotalDartsChart />
            //    </div>
            //</div>
        )
    }
}

export default Player;