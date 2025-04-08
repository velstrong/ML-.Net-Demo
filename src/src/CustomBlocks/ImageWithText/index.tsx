import { IBlock, IBlockData, BasicType, components, mergeBlock } from 'easy-email-core';
import { CustomBlocksType } from '../constants';
import React from 'react';
import { merge } from 'lodash';

const { Section, Column, Image,Text } = components;

export type IImageWithText = IBlockData<
  {
    'background-color': string;
    'text-color': string;
  },
  {
    imageUrl: string,
    quantity:number
  }
>;

export const ImageWithText: IBlock = {
  name: 'Image with text',
  type: CustomBlocksType.IMAGE_WITH_TEXT,
  create: payload => {
     const defaultData: IImageWithText = {
       type: CustomBlocksType.IMAGE_WITH_TEXT,
       data: {
        value: {
          imageUrl: 'https://yourimageshare.com/ib/fl8dUJEGiP.png',
          quantity: 11,
        },
      },
       attributes: {
         'background-color': '#4A90E2',
        'text-color': '#ffffff',
       },
       children: [
               {
                 type: BasicType.TEXT,
                 children: [],
                 data: {
                   value: {
                     content: 'custom block title',
                   },
                 },
                 attributes: {},
               },
             ],
     };
     return mergeBlock(defaultData, payload);
  },
  validParentType: [BasicType.PAGE, BasicType.WRAPPER],
  render: ({ data, idx, mode, context, dataSource }) => {
    const { imageUrl,quantity } = data.data.value;
    const attributes = data.attributes;
     return (
      <Section padding="20px"  background-color={attributes['background-color']} >
      <Column width={(100/quantity).toString()+'%'}>
        <Image padding="0px 0px 0px 0px" src={imageUrl} />
      </Column>
      <Column width={(100/12-quantity).toString()+'%'}>
      <Text
              font-size='20px'
             padding="0px 0px 0px 0px"
              line-height='1'
              align='center'
              font-weight='bold'
              color={attributes['title-color']}
            >
              test
            </Text>
      </Column>
    </Section>
        );
      },
};


export { Panel } from './Panel'