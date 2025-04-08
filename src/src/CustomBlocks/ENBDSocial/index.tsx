import {
  IBlockData,
  BasicType,
  components,
  createCustomBlock,
  getPreviewClassName,
  AdvancedType,
  mergeBlock,
} from 'easy-email-core';

import { CustomBlocksType } from '../constants';
import React from 'react';
import { Link } from 'easy-email-extensions';
import { Anchor } from 'antd';

const { Column, Section, Wrapper, Text, Button, Image, Group } = components;

export type IENBDSocial = IBlockData<
  {
    'background-color': string;
    'button-color': string;
    'button-text-color': string;
    'product-name-color': string;
    'product-price-color': string;
    'icon-width': string;
  },
  {
    title: string;
    buttonText: string;
    quantity: number;
  }
>;

export const ENBDSocial = createCustomBlock<IENBDSocial>({
  name: 'ENBD Social',
  type: CustomBlocksType.ENBD_SOCIAL,
  validParentType: [BasicType.PAGE, AdvancedType.WRAPPER, BasicType.WRAPPER,BasicType.COLUMN],
  create: payload => {
    const defaultData: IENBDSocial = {
      type: CustomBlocksType.ENBD_SOCIAL,
      data: {
        value: {
          title: 'You might also like',
          buttonText: 'Buy now',
          quantity: 3,
        },
      },
      attributes: {
        'background-color': '#d4d5d9',
        'button-text-color': '#ffffff',
        'button-color': '#414141',
        'product-name-color': '#414141',
        'product-price-color': '#414141',
        'icon-width': '100px',
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
  render: ({ data, idx, mode, context, dataSource }) => {
    const { title, buttonText, quantity } = data.data.value;
    const attributes = data.attributes;

    const perWidth = quantity <= 3 ? '' : '33.33%';

    return (
        <Section padding="12px 0px 10px 0px"  background-color={attributes['background-color']}>
        <Column width={perWidth}>
          <Image align='center' href="https://www.facebook.com/EmiratesNBD" target='blank' padding="0px 10px  0px 0px"  src="https://s.magecdn.com/social/mb-facebook.svg" />
        </Column>
        <Column  width={perWidth}>
          <Image align='center' href="https://twitter.com/EmiratesNBD_AE" target='blank' padding="0px 10px  0px 0px"  src="https://s.magecdn.com/social/mb-x.svg" />
          </Column>
          <Column  width={perWidth}>
          <Image align='center' href="https://www.instagram.com/emiratesnbd_ae/" target='blank' padding="0px 10px  0px 0px"  src="https://s.magecdn.com/social/mb-instagram.svg" />
          </Column>
          <Column   width={perWidth}>
          <Image align='center' href="https://www.youtube.com/user/EmiratesNBDChannel" target='blank' padding="0px 10px 0px 0px"  src="https://s.magecdn.com/social/mb-youtube.svg" />
          </Column>
          <Column  width={perWidth}>
          <Image align='center' href="https://www.linkedin.com/company/emirates-nbd" target='blank' padding="0px 10px 0px 0px" src="https://s.magecdn.com/social/mb-linkedin.svg" />
          </Column>
          <Column  width={perWidth}>
          <Image align='center' href="https://www.snapchat.com/add/emirates_nbd" target='blank' padding="0px 10px 0px 0px" src="https://s.magecdn.com/social/mb-pinterest.svg" />
          </Column>
      </Section>
    );
  },
});

export { Panel } from './Panel';
