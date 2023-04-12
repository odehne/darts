import React from 'react';
import { useEffect, useState } from 'react';
import { PointElement, LineElement, CategoryScale, Chart } from "chart.js";
import { Bar } from 'react-chartjs-2';

Chart.register(CategoryScale);
Chart.register(PointElement);
Chart.register(LineElement);

function HighestThrows(props) {

    const [checkoutData, setCheckoutData] = useState({
        labels: [],
        datasets: []
    });

    useEffect(() => {
        async function fetchData() {
            let mounted = true;
            let url = 'https://localhost:7141/players/' + props.playerId + '/legs/' + props.startValue + '/counthighthrows';
            await fetch(url)
                .then(response => response.json())
                .then(response => {
                    if (mounted) {
                        setCheckoutData(response)
                    }
                })
                .catch(error => console.log('error: ' + error))
            return () => mounted = false;
        }
        fetchData();
    }, [])

    return (
        <div className='mychart'>
            <h3>
                Anzahl W&uuml;rfe &uuml;ber ({props.startValue})
            </h3>
            <Bar
                data={checkoutData}
                width={300}
                height={200}
                options={{}}
            />
        </div>
    )
}

export default HighestThrows;
