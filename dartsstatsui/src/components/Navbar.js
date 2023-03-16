import React, {Component} from 'react';
import * as FaIcons from 'react-icons/fa';
import * as AiIcons from 'react-icons/ai';
import * as IoIcons from 'react-icons/io';
import { Link } from 'react-router-dom';
import './Navbar.css';
import { IconContext } from 'react-icons';


function Navbar(props) {

    const [sidebarData, setSidebarData] = useState({
        sidebar: false,
        sidebarData: {
            players: [
                {
                    name: 'olli',
                    id: 'myid',
                    path: '/player'
                }
            ],
        }
    });

    useEffect(() => {
        async function fetchData() {
            let mounted = true;
            let url = 'https://localhost:7141/players/';
            await fetch(url)
                .then(response => response.json())
                .then(response => {
                    if (mounted) {
                        setSidebarData(response)
                    }
                })
                .catch(error => console.log('error: ' + error))
            return () => mounted = false;
        }
        fetchData();
    }, [])

    return (
        <>
            <IconContext.Provider value={{ color: '#fff' }}>
                <div className="navbar">
                    <link to="#" className="menu-bars"></link>
                    <FaIcons.FaBars onClick={() => this.setState({ sidebar: !this.state.sidebar })} />
                </div>
                <nav className={this.state.sidebar ? 'nav-menu active' : 'nav-menu'} >
                    <ul className='nav-menu-items' onClick={() => this.setState({ sidebar: !this.state.sidebar })}>
                        <li className='navbar-toggle'>
                            <Link to='#' className='menu-bars'>
                                <AiIcons.AiOutlineClose />
                            </Link>
                        </li>
                        {this.state.sidebarData.players.map((player) => {
                            return (
                                <li key={player.id} className='nav-text'>
                                    <Link to={player.path}>
                                        <IoIcons.IoMdPeople />
                                        <span>{player.name}</span>
                                    </Link>
                                </li>
                            )
                        })}
                    </ul>
                </nav>
            </IconContext.Provider>
        </>
    )
}

export default Navbar;