import { BlockManager } from 'easy-email-core';
import { BlockAttributeConfigurationManager, BlockMarketManager, BlockMaskWrapper } from 'easy-email-extensions';
import { Panel as ENBDSocialPanel, ENBDSocial } from './ENBDSocial';
import { Panel as ProductRecommendationPanel, ProductRecommendation } from './ProductRecommendation';
import React from 'react';
import { CustomBlocksType } from './constants';
import { MyFirstBlock, Panel } from './MyFirstBlock';
import { ImageWithText, Panel as ImageWithTextPanel } from './ImageWithText';

BlockManager.registerBlocks({ [CustomBlocksType.MY_FIRST_BLOCK]: MyFirstBlock 
  ,[CustomBlocksType.ENBD_SOCIAL]: ENBDSocial,[CustomBlocksType.IMAGE_WITH_TEXT]: ImageWithText,[CustomBlocksType.PRODUCT_RECOMMENDATION]: ProductRecommendation});


BlockAttributeConfigurationManager.add({
  [CustomBlocksType.MY_FIRST_BLOCK]: Panel,
  [CustomBlocksType.ENBD_SOCIAL]: ENBDSocialPanel,
  [CustomBlocksType.IMAGE_WITH_TEXT]: ImageWithTextPanel,
  [CustomBlocksType.PRODUCT_RECOMMENDATION]: ProductRecommendationPanel
});

BlockMarketManager.addCategories([
  {
    title: 'Custom',
    name: 'Custom',
    blocks: [
      {
        type: CustomBlocksType.MY_FIRST_BLOCK,
        title: 'My first block',
        description: 'An custom block',
        component: () => (
          <BlockMaskWrapper
            type={CustomBlocksType.MY_FIRST_BLOCK as any}
            payload={{

            }}
          >
            <div style={{ position: 'relative' }}>
              <img src={'https://assets.maocanhua.cn/5631c12e-5788-40fd-92fe-23930a5985d7-image.png'} style={{ maxWidth: '100%' }} />
              <div style={{ position: 'absolute', top: 0, left: 0, width: '100%', height: '100%', zIndex: 2 }} />
            </div>
          </BlockMaskWrapper>
        ),
        thumbnail:
          'https://assets.maocanhua.cn/5631c12e-5788-40fd-92fe-23930a5985d7-image.png',
      },
    ],
  }
]);

