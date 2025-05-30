"use client"
import Layout from '@/Data/Layout'
import React from 'react'
import ElementLayoutCard from './ElementLayoutCard'
import ElementList from '@/Data/ElementList'
import { useDragElementLayout } from '@/app/provider'

function ElementsSideBar() {

    const { dragElementLayout, setDragElementLayout } = useDragElementLayout();
    const onDragLayoutStart = (layout) => {
        setDragElementLayout({
            dragLayout: {
                ...layout,
                id: Date.now()
            }
        })
    }

    const onDragElementStart = (element) => {
        setDragElementLayout({
            dragElement: {
                ...element,
                id: Date.now()
            }
        })
    }

    return (
        <div className='overflow-auto p-5 h-[89vh] shadow-sm'>
            <h2 className='font-bold text-lg'>Layouts</h2>
            <div className='grid grid-cols-1 md:grid-cols-2 gap-5 mt-3'>
                {Layout.map((layout, index) => (
                    <div key={index} draggable onDragStart={() => onDragLayoutStart(layout)}>
                        <ElementLayoutCard layout={layout} />
                    </div>
                ))}
            </div>

            <h2 className='font-bold text-lg mt-6'>Elements</h2>
            <div className='grid grid-cols-1 md:grid-cols-2 gap-5 mt-3'>
                {ElementList.map((element, index) => (
                    <div key={index} draggable onDragStart={() => onDragElementStart(element)}>
                        <ElementLayoutCard layout={element} />
                    </div>
                ))}
            </div>

        </div>
    )
}

export default ElementsSideBar