import React from 'react';
import { useEffect, useState } from 'react';
import { PointElement, LineElement, LinearScale, CategoryScale, Chart } from "chart.js";
import { Line } from 'react-chartjs-2';

Chart.register(CategoryScale);
Chart.register(LinearScale);
Chart.register(PointElement);
Chart.register(LineElement);

function TotalDartsChart(props) {

    const [checkoutData, setCheckoutData] = useState({
        labels: [],
        datasets: []
    });

    useEffect(() => {
        async function fetchData() {
            let mounted = true;
            let url = 'https://localhost:7141/players/' + props.playerId + '/legs/darts';
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
                Anzahl Darts pro Leg
            </h3>
            <Line
                data={checkoutData}
                width={300}
                height={200}
                options={{}}
            />
        </div>
    )
}

export default TotalDartsChart;


//class TotalDartsChart extends Component {
//    constructor(props) {
//        super(props);
//        this.state = {
//            playerId: props.playerId,
//            totalDartsData: {
//                labels: [],
//                datasets: [],
//            }
//        }
//    }

//    componentDidMount() {
//        this.populateTotalDartsData();
//    }

//    render() {
//        return (
//            <div className="mychart">
//                <Line
//                    data={this.state.totalDartsData}
//                    width={700}
//                    height={500}
//                    optionas={{}}
//                />
//            </div>
//        )
//    }

//    async populateTotalDartsData() {
//        const response = await fetch('https://localhost:7141/players/' + this.state.playerId + '/legs/darts');
//        const data = await response.json();
//        this.setState({ totalDartsData: data, loading: false });
//    }
//}

//export default TotalDartsChart;