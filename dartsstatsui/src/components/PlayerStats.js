import React from 'react';
import { useEffect, useState } from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';


function PlayerStats(props) {

    const [playerStats, setplayerStats] = useState({
        id:'',
        name: '',
        allLegAvg501: 0,
        allLegAvg301: 0,
        allLegAvg170: 0,
        bestLegAvg501: 0,
        bestLegAvg301: 0,
        bestLegAvg170: 0,
        highScore170: 0,
        highScore301: 0,
        highScore501: 0,
        bestDartCount170: 0,
        bestDartCount301: 0,
        bestDartCount501: 0,
        highestCheckout170: 0,
        highestCheckout301: 0,
        highestCheckout501: 0,
         matchCount: 0,
        legCount170: 0,
        legCount301: 0,
        legCount501: 0
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
        <div>
        <TableContainer component={Paper}>
          <Table sx={{ minWidth: 650 }} aria-label="simple table">
            <TableHead>
              <TableRow>
                <TableCell>Spielsystem</TableCell>
                <TableCell align="right">Spiele</TableCell>
                <TableCell align="right">W&uuml;rfe ∅</TableCell>
                <TableCell align="right">Bester ∅</TableCell>
                <TableCell align="right">High Score</TableCell>
                <TableCell align="right">Kleinste Wurfzahl</TableCell>
                <TableCell align="right">Bester Checkout</TableCell>
               <TableCell align="right">Legs</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
               <TableRow>
                    <TableCell component="th" scope="row">170</TableCell>
                    <TableCell align="right">{playerStats.matchCount}</TableCell>
                    <TableCell align="right">{playerStats.allLegAvg170}</TableCell>
                    <TableCell align="right">{playerStats.bestLegAvg170}</TableCell>
                    <TableCell align="right">{playerStats.highScore170}</TableCell>
                    <TableCell align="right">{playerStats.bestDartCount170}</TableCell>
                    <TableCell align="right">{playerStats.highestCheckout170}</TableCell>
                    <TableCell align="right">{playerStats.legCount170}</TableCell>
               </TableRow>
                        <TableRow>
                            <TableCell component="th" scope="row">301</TableCell>
                            <TableCell align="right">{playerStats.matchCount}</TableCell>
                            <TableCell align="right">{playerStats.allLegAvg301}</TableCell>
                            <TableCell align="right">{playerStats.bestLegAvg301}</TableCell>
                            <TableCell align="right">{playerStats.highScore301}</TableCell>
                            <TableCell align="right">{playerStats.bestDartCount301}</TableCell>
                            <TableCell align="right">{playerStats.highestCheckout301}</TableCell>
                            <TableCell align="right">{playerStats.legCount301}</TableCell>
                        </TableRow>
                        <TableRow>
                            <TableCell component="th" scope="row">501</TableCell>
                            <TableCell align="right">{playerStats.matchCount}</TableCell>
                            <TableCell align="right">{playerStats.allLegAvg501}</TableCell>
                            <TableCell align="right">{playerStats.bestLegAvg501}</TableCell>
                            <TableCell align="right">{playerStats.highScore501}</TableCell>
                            <TableCell align="right">{playerStats.bestDartCount501}</TableCell>
                            <TableCell align="right">{playerStats.highestCheckout501}</TableCell>
                            <TableCell align="right">{playerStats.legCount501}</TableCell>
                        </TableRow>
            </TableBody>
          </Table>
            </TableContainer>
        </div>
    )
}

export default PlayerStats;
