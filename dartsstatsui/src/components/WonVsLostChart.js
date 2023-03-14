import React from 'react';
import { useEffect, useState } from 'react';
import { BarElement, Title,  ArcElement, CategoryScale, Tooltip, Legend, Chart } from "chart.js";
import { Pie } from 'react-chartjs-2';

Chart.register(
    CategoryScale,
    BarElement,
    ArcElement,
    Title,
    Tooltip,
    Legend
);

function WonVsLostChart(props) {

    const [checkoutData, setCheckoutData] = useState({
        labels: [],
        datasets: []
    });

    useEffect(() => {
        async function fetchData() {
            let mounted = true;
            let url = 'https://localhost:7141/players/' + props.playerId + '/legs/winsandlosses';
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
                Gewonnene und verlorene Legs
            </h3>
            <Pie
                data={checkoutData}
                width={300}
                height={200}
                options={{}}
            />
        </div>
    )
}

export default WonVsLostChart;

//class WonVsLostChart extends Component {
//    constructor(props) {
//        super(props);
//        this.state = {
//            playerId: props.playerId,
//            wonsVsLosses: {
//                labels: [],
//                datasets: [],
//            }
//        }
//    }

//    componentDidMount() {
//        this.populateWinsAndLossesData();
//    }

//    render() {
//        return (
//            <div className="wonvslostchart">
//                <Pie
//                    data={this.state.wonsVsLosses}
//                    width={300}
//                    height={200}
//                    options={{}}
//                />
//            </div>
//        )
//    }

//    async populateWinsAndLossesData() {
//        const response = await fetch('https://localhost:7141/players/' + this.state.playerId + '/legs/winsandlosses');
//        const data = await response.json();
//        this.setState({ wonsVsLosses: data, loading: false });
//    }
//}

//export default WonVsLostChart;