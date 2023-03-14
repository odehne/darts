import React from 'react';
import { useEffect, useState } from 'react'
import '../App.css';


function Home(props) {
    const [checkoutData, setCheckoutData] = useState([]);

    useEffect(() => {
        fetch('https://localhost:7141/players/fb3fc2b2-a01b-4dd6-99e9-838262a8a614/checkouts/history')
            .then(response => response.json())
            .then(response => setCheckoutData(response))
                .catch(error => console.log(error))
    }, []);

    console.log('checkoutData: [' + checkoutData.playerId + ']')

    return (
        <div className='App-header'>
            <div className="mdc-layout-grid">
                <div className="mdc-layout-grid__inner">
                    <div className="mdc-layout-grid__cell mdc-layout-grid__cell--span-3-desktop">Hello {checkoutData.playerName}</div>
                </div>
            </div>
        </div>
    )
}

export default Home;