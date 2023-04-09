import React from 'react';
import { useEffect, useState } from 'react';
import { PointElement, LineElement, LinearScale, CategoryScale, Chart } from "chart.js";
import { Line } from 'react-chartjs-2';

Chart.register(CategoryScale);
Chart.register(LinearScale);
Chart.register(PointElement);
Chart.register(LineElement);

function BestLegChart(props) {

    const [bestLegData, setBestLegData] = useState({
        labels: [],
        datasets: []
    });

    useEffect(() => {
        async function fetchData() {
            let mounted = true;
            let url = 'https://localhost:7141/players/' + props.playerId + '/legs/' + props.startValue + '/bestleg';
            await fetch(url)
                .then(response => response.json())
                .then(response => {
                    if (mounted) {
                        setBestLegData(response)
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
                Bestes Leg ({props.startValue})
            </h3>
            <Line
                data={bestLegData}
                width={300}
                height={200}
                options={{}}
            />
        </div>
    )
}

export default BestLegChart;
