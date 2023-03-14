import React from 'react';
import { useEffect, useState } from 'react';
import { PointElement, LineElement, CategoryScale, Chart } from "chart.js";
import { Bar } from 'react-chartjs-2';

Chart.register(CategoryScale);
Chart.register(PointElement);
Chart.register(LineElement);

function LegAvgChart(props) {

    const [checkoutData, setCheckoutData] = useState({
        labels: [],
        datasets: []
    });

    useEffect(() => {
        async function fetchData() {
            let mounted = true;
            let url = 'https://localhost:7141/players/' + props.playerId + '/legs/avg';
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
                Durchschnittlicher Wurf pro Leg
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

export default LegAvgChart;


//class LegAvgChart extends Component {
//    constructor(props) {
//        super(props);
//        this.state = {
//            playerId: props.playerId,
//            checkouts: {
//                labels: [],
//                datasets: [],
//            }
//        }
//    }

//    componentDidMount() {
//        this.populateLegAvgData();
//    }

//    render() {
//        return (
//            <div className="mychart">
//                <Bar
//                    data={this.state.checkouts}
//                    width={700}
//                    height={500}
//                    optionas={{}}
//                />
//            </div>
//        )
//    }

//    async populateLegAvgData() {
//        const response = await fetch('https://localhost:7141/players/' + this.state.playerId + '/legs/avg');
//        const data = await response.json();
//        this.setState({ checkouts: data, loading: false });
//    }
//}

//export default LegAvgChart;