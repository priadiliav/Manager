import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Tooltip,
    Legend,
} from "chart.js";
import { Line } from "react-chartjs-2";
import { useRef, useEffect, useState } from "react";

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Tooltip, Legend);


interface Props {
    data: number[];
    labels: string[];
}

export const CustomLineChart = ({ data, labels }: Props) => {
    const chartRef = useRef<any>(null);
    const [gradient, setGradient] = useState<string>("rgba(75,192,192,0.3)");

    useEffect(() => {
        if (chartRef.current) {
            const ctx = chartRef.current.ctx;
            const grd = ctx.createLinearGradient(0, 0, 0, ctx.canvas.height);
            grd.addColorStop(0, "rgba(75,192,192,0.6)");
            grd.addColorStop(1, "rgba(75,192,192,0)");
            setGradient(grd);
        }
    }, []);

    const normalizedData = {
        labels: labels,
        datasets: [
            {
                label: "Revenue",
                data: data,
                fill: true,
                borderColor: "rgb(75,192,192)",
                backgroundColor: gradient,
                tension: 0.3,
            },
        ],
    };

    const options = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: { display: false },
        },
        scales: {
            y: { beginAtZero: true },
        },
    };

    return <Line ref={chartRef} data={normalizedData} options={options} />;
};
