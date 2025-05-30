import React from 'react'

function ColorPickerField({ label, value, onHandleStyleChange }) {
    return (
        <div className='grid'>
            <label>{label}</label>
            <input type="color" value={value}
                onChange={(e) => onHandleStyleChange(e.target.value)} />
        </div>
    )
}

export default ColorPickerField