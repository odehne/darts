import React from 'react';
import { Box, Stack, Divider, Grid } from '@mui/material'
import '../App.css';


function Home() {
    return (
        //<div className='App-header'>
        //    <div className="mdc-layout-grid">
        //        <div className="mdc-layout-grid__inner">
        //            <div className="mdc-layout-grid__cell mdc-layout-grid__cell--span-3-desktop">home cell 1</div>
        //            <div className="mdc-layout-grid__cell mdc-layout-grid__cell--span-3-desktop">home cell 2</div>
        //            <div className="mdc-layout-grid__cell mdc-layout-grid__cell--span-3-desktop">home cell 3</div>
        //        </div>
        //    </div>
        //</div>
        <Stack sx={{ border: '1px solid', padding: '10px' }} direction='row' spacing={1} divider={<Divider orientation='vertical' flexitem />}>

            <Box
                sx={{
                    backgroundColor: 'primary.main',
                    color: 'white',
                    height: '100px',
                    width: '100px',
                    padding: '16px',
                    '&:hover': {
                        backgroundColor: 'primary.light',
                    },
                } }
            >
                Paul
            </Box>

        </Stack>

    )
}

export default Home;